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
        public ObservableRangeCollection<ItemDetailsViewModel> items = new();
        private const int pageSize = 20;
        public int currentPage = 0;
        private List<FoodData> cachedFoodData = new();
        [ObservableProperty]
        public CartDetailsViewModel selectedCart;
        public event Action<string> ErrorOccurred;

       
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
            DeleteItemCommand = new AsyncRelayCommand<ItemDetailsViewModel>(OnDeleteItemCommand);
            SelectedItemCommand = new AsyncRelayCommand<ItemDetailsViewModel>(OnCheckItemCommand);

        }

        public async Task OnInitializeDataAsync()
        {
            var result = await CartService.GetCartIdAsync(SelectedCart.Id);
            if (result != null)
            {
                var sortedItems = result.Items!.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).Select(i => new ItemDetailsViewModel(i)).ToList();
                Items.AddRange(sortedItems);
                UpdateProgress();

            }
            await LoadCacheData();
        }

        private async Task OnFoodDataPageCommandAsync()
        {

            var page = new FoodDataPage(this);

            page.Opacity = 0;
            await Shell.Current.Navigation.PushModalAsync(page, false);
            await page.FadeTo(1, 400, Easing.CubicInOut);

            await OnLoadFoodDataAsync();
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

            await Task.Run(async () =>
            {
                 var itemsToLoad = await foodDataService.GetPageDataAsync(currentPage, pageSize);
                 if (itemsToLoad.Any())
                 {
                     FoodDataItems.AddRange(itemsToLoad);
                     currentPage++;
                 }
            });


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
                OnErrorOccurred("Food data is null");
                return;
            }

            var newItem = await CartService.AddItemToCartAsync(SelectedCart.Id, foodData);
            Items.Add(new ItemDetailsViewModel(newItem));
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#39de57"),
                TextColor = Color.FromArgb("#FFFFFF"),
                CornerRadius = 10,

            };
            await Snackbar.Make($"{foodData.Name} added to cart", () => { }, string.Empty, TimeSpan.FromSeconds(2), snackbarOptions).Show();

            SortItems();

        }
        private void UpdateProgress()
        {
            ProgressBar = (double)Items.Count(i => i.IsChecked) / Items.Count;
            var selectedCartUI = cartViewModel.Carts.FirstOrDefault(i => i.Id == SelectedCart.Id);
            if (selectedCartUI == null)
            {
                OnErrorOccurred("Cart not found");
                return;
            }

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
        public async Task OnCheckItemCommand(ItemDetailsViewModel item)
        {
            if (item == null)
            {
                OnErrorOccurred("Item is null");
                return;
            }
            try
            {
                item.IsChecked = !item.IsChecked;

               await CartService.UpdateItemCheckedStatusAsync(SelectedCart.Id, item.Id, item.IsChecked);
                SortItems();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                throw;
            }

        }

        private void SortItems()
        {
            var sortedItem = Items.OrderBy(i => i.IsChecked).ThenBy(i => i.ItemName).ToList();
            Items.ReplaceRange(sortedItem);
            UpdateProgress();
        }
        private async Task OnDeleteItemCommand(ItemDetailsViewModel? item)
        {
            var itemToDelete = Items.FirstOrDefault(i => i.Id == item?.Id);
            if (itemToDelete != null)
            {
                Items.Remove(itemToDelete);
                await CartService.DeleteItemFromCartAsync(SelectedCart.Id, itemToDelete.Id);
            }
            UpdateProgress();
        }

        private async Task OnCloseCommandAsync()
        {
            if (Shell.Current.CurrentPage is FoodDataPage page)
            {
                await page.FadeTo(0, 300, Easing.CubicInOut); 
                page.HideKeyboardOnDismissAsync();
                SearchText = string.Empty;
            }

            await Shell.Current.Navigation.PopModalAsync();
        }
        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }



    }
}
