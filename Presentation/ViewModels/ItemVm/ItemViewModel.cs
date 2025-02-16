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
        public ObservableRangeCollection<FoodData> _foodDataItems = new ObservableRangeCollection<FoodData>();

        [ObservableProperty]
        public ObservableRangeCollection<Item> _items = new ObservableRangeCollection<Item>();
        private const int pageSize = 30;
        private int currentPage = 0;
        private List<FoodData> cachedFoodData = new();
        [ObservableProperty]
        public CartDetailsViewModel _selectedCart;
        public event Action<string> ErrorOccurred;
        private BottomSheet currentBottomSheet;
        [ObservableProperty]
        public string _searchText;
        [ObservableProperty]
        public double _progressBar;
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
            AddAndFindItemCommand = new AsyncRelayCommand(OnSeachCommandAsync);
            LoadMoreItemsCommand = new AsyncRelayCommand(OnLoadMoreFoodDataAsync);
            CloseCommand = new AsyncRelayCommand(OnCloseCommandAsync);
            SearchItemsCommand = new AsyncRelayCommand(OnSearchItemsCommand);
            ItemSelectedCommand = new AsyncRelayCommand<FoodData>(OnSelectedItemCommand);
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
                Items.AddRange(sortedItems);
                UpdateProgress();
            }
        }
        private async Task OnSeachCommandAsync()
        {
            await OnLoadFoodDataAsync();
            await Task.Delay(1000);
            await Shell.Current.Navigation.PushAsync(new FoodDataItemsPage(this));
            //await Shell.Current.Navigation.PushModalAsync(new FoodDataItemsPage(this));

            //var addNewItemBS = new AddNewItemBottomSheet(this);
            //await addNewItemBS.ShowAsync();
            //currentBottomSheet = addNewItemBS;

        }



        private async Task OnLoadFoodDataAsync()
        {
            currentPage = 0;
            FoodDataItems.Clear();
            SearchText = string.Empty;
            await OnLoadMoreFoodDataAsync();

        }
        //private async Task OnLoadMoreFoodDataAsync()
        //{
        //    if (!string.IsNullOrWhiteSpace(SearchText))
        //    {
        //        return;
        //    }
        //    if (!cachedFoodData.Any())
        //    {
        //        await Task.Run(async () =>
        //        {
        //            var result = await foodDataService.GetAllAsync();
        //            if (result != null)
        //            {
        //                cachedFoodData = result.ToList();
        //            }
        //        });

        //    }
        //    await Task.Run(async () => FoodDataItems.AddRange(cachedFoodData.Skip(FoodDataItems.Count()).Take(pageSize)));
        //    //var itemsToLoad = cachedFoodData.Skip(FoodDataItems.Count()).Take(pageSize);
        //    //FoodDataItems.AddRange(itemsToLoad);

        //    currentPage++;

        //}
        private async Task OnLoadMoreFoodDataAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                return;
            }

            if (!cachedFoodData.Any())
            {
                var result = await Task.Run(async () => await foodDataService.GetAllAsync());
                if (result != null)
                {
                    cachedFoodData = result.ToList();
                }
            }

            var itemsToLoad = cachedFoodData.Skip(FoodDataItems.Count()).Take(pageSize).ToList();

            FoodDataItems.AddRange(itemsToLoad);

            currentPage++;
        }
        private async Task OnSelectedItemCommand(FoodData foodData)
        {
            if (foodData != null)
            {

                await cartViewModel.OnAddNewItemAsync(foodData);
                var item = FoodDataItems.FirstOrDefault(i => i.Id == foodData.Id);
                if (item != null)
                {
                    item.IsSelected = true;

                }
                Items.Add(new Item { ItemName = foodData.Name, IsChecked = false });
                var sortedItems = await Task.Run(() => Items.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).ToList());
                Items.Clear();
                Items.AddRange(sortedItems);
                UpdateProgress();
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Color.FromArgb("#39de57"),
                    TextColor = Color.FromArgb("#FFFFFF"),
                    CornerRadius = 10,

                };
                await Snackbar.Make($"{foodData.Name} added to cart", () => { }, string.Empty, TimeSpan.FromSeconds(2), snackbarOptions).Show();
                //Items.AddRange(sortedItems);

            }
        }
        private void UpdateProgress()
        {
            ProgressBar = (double)Items.Count(i => i.IsChecked) / Items.Count;

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
            item.IsChecked = !item.IsChecked;
            var selectedCartUI = cartViewModel.Carts.FirstOrDefault(i => i.Id == SelectedCart.Id);
            if(selectedCartUI == null) return;

            selectedCartUI.Progressing = await cartViewModel.CalculateProgress(Items.Count(i => i.IsChecked), Items.Count);

            await Task.Run(async () =>
            {
                var dbCart = await CartService.GetCartIdAsync(SelectedCart.Id);
              var dbItem = dbCart.Items!.FirstOrDefault(i => i.Id == item.Id);
                if (dbItem != null)
                {
                    dbItem.IsChecked = item.IsChecked;
                    await CartService.UpdateCartAsync(dbCart);
                }
            });
            
            var sortedItems = Items.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).ToList();
            Items.Clear();
            Items.AddRange(sortedItems);
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
        //====Remove this method to reminder
        private async Task OnCloseCommandAsync()
        {
            if (currentBottomSheet != null)
            {
                var currentBS = currentBottomSheet as AddNewItemBottomSheet;
                currentBS!.HideKeyboardOnDismissAsync();
                await currentBS!.DismissAsync(true);

            }
        }
        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }



    }
}
