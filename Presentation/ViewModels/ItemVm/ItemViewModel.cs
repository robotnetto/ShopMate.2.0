using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.Views.CartView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.ViewModels.ItemVm
{
    public partial class ItemViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly FoodDataService foodDataService;
        public readonly CartViewModel cartViewModel;
        [ObservableProperty]
        public ObservableRangeCollection<FoodData> foodDataItems = new ObservableRangeCollection<FoodData>();

        [ObservableProperty]
        public ObservableRangeCollection<Item> items = new ObservableRangeCollection<Item>();
        private const int pageSize = 20;
        public int currentPage = 0;
        private List<FoodData> cachedFoodData = new();
        [ObservableProperty]
        public CartDetailsViewModel selectedCart;
        public event Action<string> ErrorOccurred;

        public BottomSheet CurrentBottomSheet { get; set; }
        [ObservableProperty]
        public string _searchText;
        [ObservableProperty]
        public double _progressBar;
        [ObservableProperty]
        public bool _isLoading;


        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
        public ICommand ItemSelectedCommand { get; }
        public ICommand AddAndFindItemCommand { get; }

        public ICommand LoadMoreItemsCommand { get; set; }
        public ICommand CloseCommand { get; }
        public ICommand DebounceSearchItemsCommand { get; }
        public ICommand SearchItemsCommand { get; }
        public ICommand SelectedItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public CartService CartService { get; }


        public ItemViewModel(FoodDataService foodDataService, CartViewModel cartVM, CartService cartService)
        {
            this.foodDataService = foodDataService;
            cartViewModel = cartVM;
            SelectedCart = cartVM.SelectedCart;
            this.CartService = cartService;
            AddAndFindItemCommand = new AsyncRelayCommand(OnFoodDataPageCommandAsync);
            LoadMoreItemsCommand = new AsyncRelayCommand(OnLoadMoreFoodDataAsync);
            CloseCommand = new AsyncRelayCommand(OnCloseCommandAsync);
            SearchItemsCommand = new AsyncRelayCommand(OnSearchItemsCommand);
            ItemSelectedCommand = new AsyncRelayCommand<FoodData>(OnAddNewItemCommand);
            DebounceSearchItemsCommand = new DebounceCommand(SearchItemsCommand, TimeSpan.FromMilliseconds(200));
            DeleteItemCommand = new AsyncRelayCommand<Item>(OnDeleteItemCommand);
            SelectedItemCommand = new AsyncRelayCommand<Item>(OnCheckItemCommand);
           
        }


        //public ItemViewModel() : this(new FoodDataService(new FoodDataRepository(new LocalDbService())))
        //{
        //}

        public async Task OnInitializeDataAsync()
        {
            var result = await CartService.GetCartIdAsync(SelectedCart.Id);
            if (result != null)
            {
                var sortedItems = result.Items!.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).ToList();
                Items.Clear();
                Items.AddRange(sortedItems);
                UpdateProgress();

            }
            await LoadCacheData();
        }

        private async Task OnFoodDataPageCommandAsync()
        {
            await OnLoadFoodDataAsync();
            //await Shell.Current.Navigation.PushAsync(new FoodDataPage(this), true);
            //await Shell.Current.GoToAsync(nameof(FoodDataPage), true);
            var bottomSheet = new AddNewItemBottomSheet(this);
            CurrentBottomSheet = bottomSheet;

            await bottomSheet.ShowAsync();


        }


        public async Task OnLoadFoodDataAsync()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                IsLoading = true;
                currentPage = 0;
                FoodDataItems.Clear();
                SearchText = string.Empty;


                await OnLoadMoreFoodDataAsync();
            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                IsLoading = false;
                semaphoreSlim.Release();
            }

        }

        private async Task OnLoadMoreFoodDataAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                return;
            }

            var itemsToLoad = await foodDataService.GetPageDataAsync(currentPage, pageSize);
            if (itemsToLoad.Any())
            {
                FoodDataItems.AddRange(itemsToLoad);
                currentPage++;
            }



        }
        private async Task LoadCacheData()
        {
            if (!cachedFoodData.Any())
            {
                await Task.Run(async () =>
                {
                    var allData = await foodDataService.GetAllAsync();
                    if (allData != null)
                    {
                        cachedFoodData = allData.ToList();
                    }
                });
            }
        }
        private async Task OnAddNewItemCommand(FoodData foodData)
        {
            if (foodData == null)
            {
                return;
            }

            var newItem = new Item { ItemName = foodData.Name, IsChecked = false , CartId = selectedCart.Id};
            Items.Add(newItem);
            SortItems();
            var dbCart = await CartService.GetCartIdAsync(SelectedCart.Id);
            if (dbCart != null)
            {
                dbCart.Items!.Add(newItem);
                await CartService.UpdateCartAsync(dbCart);
            }
            //var snackbarOptions = new SnackbarOptions
            //{
            //    BackgroundColor = Color.FromArgb("#39de57"),
            //    TextColor = Color.FromArgb("#FFFFFF"),
            //    CornerRadius = 10,

            //};
            //await Snackbar.Make($"{foodData.Name} added to cart", () => { }, string.Empty, TimeSpan.FromSeconds(2), snackbarOptions).Show();

            var toast = Toast.Make($"{foodData.Name} added to your cart", ToastDuration.Short, 14);
            await toast.Show();



        }
        private void UpdateProgress()
        {
            ProgressBar = (double)Items.Count(i => i.IsChecked) / Items.Count;
            var selectedCartUI = cartViewModel.Carts.FirstOrDefault(i => i.Id == SelectedCart.Id);
            if (selectedCartUI == null) return;

            selectedCartUI.Progressing = ProgressBar;

            OnPropertyChanged(nameof(ProgressBar));
        }
        private async Task OnSearchItemsCommand()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await OnLoadFoodDataAsync();
                return;
            }
            var result = cachedFoodData.Where(f => f.Name.StartsWith(SearchText, StringComparison.OrdinalIgnoreCase)).Take(50).ToList();

            FoodDataItems.Clear();
            if (result.Any())
            {
                FoodDataItems.AddRange(result);
            }
            else
            {
                FoodDataItems.Add(new FoodData { Name = SearchText });
            }

        }
        public async Task OnCheckItemCommand(Item item)
        {
            if (item == null)
            {
                return;
            }
            try
            {
                item.IsChecked = !item.IsChecked;

                SortItems();
                var dbCart = await CartService.GetCartIdAsync(SelectedCart.Id);

                var dbItem = dbCart.Items!.FirstOrDefault(i => i.Id == item.Id);
                if (dbItem != null)
                {

                    dbItem.IsChecked = item.IsChecked;
                    await CartService.UpdateCartAsync(dbCart);
                }

            }
            catch (Exception e)
            {

                throw;
            }

        }

        private void SortItems()
        {
            var sortedItem =  Items.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).ToList();
            Items.ReplaceRange(sortedItem);
            UpdateProgress();
        }
        private async Task OnDeleteItemCommand(Item? item)
        {
            var itemToDelete = Items.FirstOrDefault(i => i.Id == item?.Id);
            if (itemToDelete != null)
            {
                Items.Remove(itemToDelete);
                var cartFromDb = await CartService.GetCartIdAsync(SelectedCart.Id);
                var dbItem = cartFromDb.Items!.FirstOrDefault(i => i.Id == itemToDelete.Id);
                if (dbItem != null)
                {
                    cartFromDb.Items!.Remove(dbItem);
                    await CartService.UpdateCartAsync(cartFromDb);
                }
            }
            UpdateProgress();
        }

        private async Task OnCloseCommandAsync()
        {
            if (CurrentBottomSheet != null)
            {
                await CurrentBottomSheet.DismissAsync();
            }
        }
        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }



    }
}
