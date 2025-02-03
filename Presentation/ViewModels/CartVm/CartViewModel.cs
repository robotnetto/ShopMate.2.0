using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.Enum;
using System.Collections.ObjectModel;
using ShopMate._2._0.Presentation.Views.CartView;
using System.Windows.Input;
using The49.Maui.BottomSheet;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Interfaces;
using System.Diagnostics;
using System.Text;


namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly CartService shopCartService;

        [ObservableProperty]
        public ObservableCollection<CartDetailsViewModel> _carts = new();

        [ObservableProperty]
        public string _titleName;

        [ObservableProperty]
        public string _itemName;

        [ObservableProperty]
        public CartDetailsViewModel _selectedCart;

        [ObservableProperty]
        public string _bottomSheetTitle;

        //public ObservableCollection<Item> SelectedCartItems;
        //=> new ObservableCollection<Item>(_selectedCart.Items);

        public event Action<string>? ErrorOccurred;
        private BottomSheetMode CurrentBottomSheetMode { get; set; }
        private BottomSheet CurrentBottomSheet { get; set; }
        public ICommand CardSelectCommand { get; }
        public ICommand AddCartCommand { get; }
        public ICommand SaveCartCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand DeleteCartCommand { get; }
        public ICommand EditCartCommand { get; }
        public ICommand ShareListCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddItemComand { get; }
        private CartViewModel(CartService shopCartService)
        {
            this.shopCartService = shopCartService ?? throw new ArgumentNullException(nameof(shopCartService));
            AddCartCommand = new AsyncRelayCommand(OnAddCartCommandAsync);
            CardSelectCommand = new AsyncRelayCommand<CartDetailsViewModel>(OnNavigateAsync);
            SaveCartCommand = new AsyncRelayCommand(OnSaveRecipeAndEditCartCommandAsync);
            OptionsCommand = new AsyncRelayCommand<CartDetailsViewModel>(OnOptionsCommand);
            DeleteCartCommand = new AsyncRelayCommand(OnDeleteCartCommandAsync);
            EditCartCommand = new AsyncRelayCommand(OnEditCartCommandAsync);
            ShareListCommand = new AsyncRelayCommand(OnShareList);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheetAsync);
            AddItemComand = new AsyncRelayCommand(OnAddItemCommandAsync);
            _ = InitializeDataAsync();
        }



        public CartViewModel() : this(new CartService(new CartRepository(new Infrastructure.Data.LocalDbService())))
        {
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var results = await shopCartService.GetAllCartsServiceAsync();


                foreach (var item in results)
                {
                    Carts.Add(new CartDetailsViewModel(item));

                }
            }
            catch (Exception e)
            {

                OnErrorOccurred(e.Message);
            }

        }

        private async Task OnLoadItemAsync()
        {
            var result = await shopCartService.GetCartIdAsync(SelectedCart.Id);
            if (result != null)
            {
                var cart = Carts.FirstOrDefault(c => c.Id == result.Id);
                if (cart != null)
                {
                    SelectedCart.Items = new ObservableCollection<Item>(result.Items!);
                }
            }

        }
        private async Task OnSaveRecipeAndEditCartCommandAsync()
        {
            if (CurrentBottomSheetMode == BottomSheetMode.Add)
            {
                await OnAddNewCartAsync();
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.Edit)
            {
                await OnUpdateCartAsync();
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.AddItem)
            {
                await OnAddNewItemAsync();
            }
        }
        private async Task OnOptionsCommand(CartDetailsViewModel cartDetailsViewModel)
        {

            SelectedCart = cartDetailsViewModel;
            var bottomSheet = new CartOptionsBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;
            CurrentBottomSheetMode = BottomSheetMode.Options;
            TitleName = SelectedCart.Title;
        }


        private async Task OnAddNewCartAsync()
        {
            Cart cart = new() { Title = TitleName, Items = new List<Item>() };
            await shopCartService.AddNewCartAsync(cart);
            var shopCartVm = new CartDetailsViewModel(cart);
            Carts.Add(shopCartVm);
            TitleName = string.Empty;
            await OnCloseBottomSheetAsync();
        }

        private async Task OnAddNewItemAsync()
        {
            if (string.IsNullOrWhiteSpace(TitleName))
            {
                OnErrorOccurred("Item name can't be empty or whitespace!");
                return;
            }
            var result = await shopCartService.GetCartIdAsync(SelectedCart.Id);
            if (result != null)
            {
                var newItem = new Item { ItemName = TitleName.Trim(), IsChecked = false };

                result.Items!.Add(newItem);
                await shopCartService.UpdateCartAsync(result);

                var cartVm = Carts.FirstOrDefault(c => c.Id == result.Id);

                if (cartVm != null)
                {
                    SelectedCart.Items.Add(newItem);
                }
                TitleName = string.Empty;
                await OnCloseBottomSheetAsync();
            }
        }
        private async Task OnUpdateCartAsync()
        {
            var cart = await shopCartService.GetCartIdAsync(SelectedCart.Id);
            if (cart != null)
            {
                cart.Title = TitleName!;
                await shopCartService.UpdateCartAsync(cart);
                var cartVm = Carts.FirstOrDefault(c => c.Id == cart.Id);
                if (cartVm != null)
                {
                    cartVm.Title = cart.Title;

                }
                TitleName = string.Empty;
                await OnCloseBottomSheetAsync();

            }

        }
        private async Task OnDeleteCartCommandAsync()
        {
            var cart = await shopCartService.GetCartIdAsync(SelectedCart.Id);
            if (cart is not null)
            {
                await shopCartService.DeleteCartAsync(cart);
                var cartVm = Carts.FirstOrDefault(c => c.Id == cart.Id);
                Carts.Remove(cartVm!);
            }
            await OnCloseBottomSheetAsync();
        }
        private async Task OnAddCartCommandAsync()
        {
            await ShowButtomSheetAsync(BottomSheetMode.Add);
        }
        private async Task OnEditCartCommandAsync()
        {
            await CurrentBottomSheet!.DismissAsync(true);
            await ShowButtomSheetAsync(BottomSheetMode.Edit);
        }
        private async Task OnAddItemCommandAsync()
        {
            await ShowButtomSheetAsync(BottomSheetMode.AddItem);
        }
        private async Task ShowButtomSheetAsync(BottomSheetMode mode)
        {
            if (mode == BottomSheetMode.AddItem)
            {
                CurrentBottomSheetMode = mode;
                BottomSheetTitle = "Add Item";
                var bottomSheet = new AddNewItem(this);
                await bottomSheet.ShowAsync();
               
                CurrentBottomSheet = bottomSheet;

            }
            else
            {
                TitleName = mode == BottomSheetMode.Add ? string.Empty : SelectedCart.Title ?? string.Empty;
                CurrentBottomSheetMode = mode;
                BottomSheetTitle = mode == BottomSheetMode.Add ? "New" : "Change name";
                var bottomSheet = new CartAddEditBottomSheet(this);
                await bottomSheet.ShowAsync();
                CurrentBottomSheet = bottomSheet;
            }

        }
        private async Task OnCloseBottomSheetAsync()
        {
            if (CurrentBottomSheetMode == BottomSheetMode.Add 
                || CurrentBottomSheetMode == BottomSheetMode.Edit    )
            {
                var buttomSheet = CurrentBottomSheet as CartAddEditBottomSheet;
                buttomSheet!.CartHideKeyboardAsync();
                await buttomSheet!.DismissAsync(true);
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.AddItem)
            {
                var buttomSheet = CurrentBottomSheet as AddNewItem;
                buttomSheet!.HideKeyboardOnDismissAsync();
                await buttomSheet!.DismissAsync(true);
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.Options)
            {
                var buttomSheet = CurrentBottomSheet as CartOptionsBottomSheet;
                await buttomSheet!.DismissAsync(true);
            }

        }
        //private void OnSelectedCartChanged()
        //{
        //    if (_selectedCart != null)
        //    {
        //        // Sort the items in SelectedCart whenever an item is checked/unchecked
        //        foreach (var item in _selectedCart.Items)
        //        {
        //            item.PropertyChanged += (sender, args) =>
        //            {
        //                if (args.PropertyName == nameof(Item.IsChecked))
        //                {
        //                    SortItems();
        //                    OnPropertyChanged(nameof(SelectedCartItems)); // Trigger UI update
        //                }
        //            };
        //        }
        //    }
        //}

        private void SortItems()
        {
            SelectedCart.Items = new ObservableCollection<Item>(SelectedCart.Items.OrderBy(i => i.IsChecked).ThenBy(n => n.ItemName).ToList());

        }
        private async Task OnShareList()
        {
            var itemList = new StringBuilder();
            itemList.AppendLine($"{SelectedCart.Title}");
            itemList.AppendLine("Items:");

            foreach (var item in SelectedCart.Items)
            {
                itemList.AppendLine($"- {item.ItemName}");
            }

            await Share.RequestAsync(new ShareTextRequest
            {

                Text = itemList.ToString()
            });


        }
        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }
        private async Task OnNavigateAsync(CartDetailsViewModel cartDetailsViewModel)
        {
            SelectedCart = cartDetailsViewModel;
            await OnLoadItemAsync();
            await Shell.Current.Navigation.PushAsync(new ItemPage(this));

        }
    }
}
