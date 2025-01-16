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

namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ShopCartService shopCartService;

        [ObservableProperty]
        public ObservableCollection<CartDetailsViewModel> shopCarts = new();
        [ObservableProperty]
        public string _cartNameTitle;
        [ObservableProperty]
        public CartDetailsViewModel _selectedCart;
        [ObservableProperty]
        public string _bottomSheetTitle;
        private BottomSheetMode CurrentBottomSheetMode { get; set; }
        private BottomSheet CurrentBottomSheet { get; set; }
        public ICommand AddCartCommand { get; }
        public ICommand SaveCartCommand { get; }
        public ICommand OptionsCommand { get; }


        public CartViewModel(ShopCartService shopCartService)
        {
            this.shopCartService = shopCartService ?? throw new ArgumentNullException(nameof(shopCartService));
            AddCartCommand = new AsyncRelayCommand(OnAddCartCommand);
            SaveCartCommand = new AsyncRelayCommand(OnSaveRecipeAndEditCartCommand);
            OptionsCommand = new AsyncRelayCommand<CartDetailsViewModel>(OnOptionsCommand);
           _ = InitializeDataAsync();
        }

        public CartViewModel(): this(new ShopCartService(new ShopCartRepository( new Infrastructure.Data.LocalDbService())))
        {
        }
        private async Task OnOptionsCommand(CartDetailsViewModel cartDetailsViewModel)
        {
            SelectedCart = cartDetailsViewModel;
            var bottomSheet = new CartOptionsBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;
            CurrentBottomSheetMode = BottomSheetMode.Options;
            CartNameTitle = SelectedCart.Title;
        }


        public async Task InitializeDataAsync()
        {
            var results = await shopCartService.GetAllCarts();
            foreach (var item in results)
            {
                var cartsVm = new CartDetailsViewModel(item);
                ShopCarts.Add(cartsVm);
            }

        }
        private async Task OnSaveRecipeAndEditCartCommand()
        {
            if (CurrentBottomSheetMode == BottomSheetMode.Add)
            {
                await OnSaveCart();
            }
            throw new NotImplementedException();
        }

        private async Task OnSaveCart()
        {
            ShopCart cart = new() { Title = CartNameTitle, Items = new List<Item>() };
            await shopCartService.AddNewCart(cart);
            var shopCartVm = new CartDetailsViewModel(cart);
            ShopCarts.Add(shopCartVm);
        }

        private async Task OnAddCartCommand()
        {
            await ShowButtomSheet(BottomSheetMode.Add);
        }
        private async Task ShowButtomSheet(BottomSheetMode mode)
        {
            CartNameTitle = mode == BottomSheetMode.Add? string.Empty : SelectedCart.Title?? string.Empty;
            CurrentBottomSheetMode = mode;
            BottomSheetTitle =  mode == BottomSheetMode.Add ? "New" : "Change name";
            var bottomSheet = new CartAddEditBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;
        }
    }
}
