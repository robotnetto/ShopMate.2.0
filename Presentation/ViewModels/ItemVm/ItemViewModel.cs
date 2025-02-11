using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.Views.CartView;
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
        private const int pageSize = 20;
        private int currentPage = 0;
        private List<FoodData> cachedFoodData = new();
        [ObservableProperty]
        public CartDetailsViewModel _selectedCart;
        public event Action<string> ErrorOccurred;
        private BottomSheet currentBottomSheet;
        [ObservableProperty]
        public string _searchText;

        public ICommand ItemSelectedCommand { get; }
        public ICommand AddAndSearchCommand { get; }
        public ICommand LoadMoreItemsCommand { get; set; }
        public ICommand CloseCommand { get; }
        public ICommand DebounceSearchItemsCommand { get; }
        public ICommand SearchItemsCommand { get; }
        public ICommand SelectedItemCommand { get; }

        public CartService CartService { get; }


        public ItemViewModel(FoodDataService foodDataService, CartViewModel cartVM, CartService cartService)
        {
            this.foodDataService = foodDataService;
            cartViewModel = cartVM;
            SelectedCart = cartVM.SelectedCart;
            this.CartService = cartService;
            AddAndSearchCommand = new AsyncRelayCommand(OnSeachCommandAsync);
            LoadMoreItemsCommand = new AsyncRelayCommand(OnLoadMoreFoodDataAsync);
            CloseCommand = new AsyncRelayCommand(OnCloseCommandAsync);
            SearchItemsCommand = new AsyncRelayCommand(OnSearchItemsCommand);
            ItemSelectedCommand = new AsyncRelayCommand<FoodData>(OnSelectedItemCommand);
            DebounceSearchItemsCommand = new DebounceCommand(SearchItemsCommand, TimeSpan.FromMilliseconds(200));
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
                Items.AddRange(result.Items!);
            }
        }
        private async Task OnSeachCommandAsync()
        {
            //await Shell.Current.Navigation.PushAsync(new SearchItemPage(this));
            var addNewItemBS = new AddNewItemBottomSheet(this);
            await addNewItemBS.ShowAsync();
            currentBottomSheet = addNewItemBS;
            await OnLoadFoodDataAsync();
        }



        private async Task OnLoadFoodDataAsync()
        {
            currentPage = 0;
            FoodDataItems.Clear();
            cachedFoodData.Clear();
            await OnLoadMoreFoodDataAsync();

        }
        private async Task OnLoadMoreFoodDataAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                return;
            }
            if (!cachedFoodData.Any())
            {
                var result = await foodDataService.GetAllAsync();
                if (result != null)
                {
                    cachedFoodData = result.ToList();

                }
            }
            var itemsToLoad = cachedFoodData.Skip(FoodDataItems.Count()).Take(pageSize);
            FoodDataItems.AddRange(itemsToLoad);

            currentPage++;

        }

        private async Task OnSelectedItemCommand(FoodData foodData)
        {
            if (foodData != null)
            {

                await cartViewModel.OnAddNewItemAsync(foodData);
                await Toast.Make("Item added to cart", ToastDuration.Short).Show();
            }
            Items.Clear();
            await OnInitializeDataAsync();
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
            if (item != null)
            {
                item.IsChecked = !item.IsChecked;
                var cartFromDb = await CartService.GetCartIdAsync(SelectedCart.Id);
                var dbItem = cartFromDb.Items!.FirstOrDefault(i => i.Id == item.Id);
                if (dbItem != null)
                {
                    dbItem.IsChecked = item.IsChecked;
                    await CartService.UpdateCartAsync(cartFromDb);
                }


                var checkedItems = Items.FirstOrDefault(i => i.Id == item.Id);
                if (checkedItems != null)
                {
                    checkedItems.IsChecked = item.IsChecked;
                }
            }

        }
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
