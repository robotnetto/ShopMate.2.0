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
using ShopMate._2._0.Presentation.ViewModels.ItemVm;
using Microsoft.Extensions.Logging.Abstractions;
using ShopMate._2._0.Infrastructure.Data;


namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly CartService cartService;

        [ObservableProperty]
        public ObservableCollection<CartDetailsViewModel> carts = new();

        [ObservableProperty]
        public string titleName;

        [ObservableProperty]
        public string itemName;

        [ObservableProperty]
        public CartDetailsViewModel selectedCart;

        [ObservableProperty]
        public string bottomSheetTitle;



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
        //public ICommand AddItemComand { get; }
        private CartViewModel(CartService cartService)
        {
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            AddCartCommand = new AsyncRelayCommand(OnAddCartCommandAsync);
            CardSelectCommand = new AsyncRelayCommand<CartDetailsViewModel>(OnNavigateAsync);
            SaveCartCommand = new AsyncRelayCommand(OnSaveRecipeAndEditCartCommandAsync);
            OptionsCommand = new AsyncRelayCommand<CartDetailsViewModel>(OnOptionsCommand);
            DeleteCartCommand = new AsyncRelayCommand(OnDeleteCartCommandAsync);
            EditCartCommand = new AsyncRelayCommand(OnEditCartCommandAsync);
            ShareListCommand = new AsyncRelayCommand(OnShareList);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheetAsync);
            //AddItemComand = new AsyncRelayCommand(OnAddItemCommandAsync);
            _ = OnInitializeDataAsync();
        }



        public CartViewModel() : this(new CartService(new CartRepository(new Infrastructure.Data.LocalDbService())))
        {
        }

        public async Task OnInitializeDataAsync()
        {
            try
            {
                var cartsFromDb = await cartService.GetAllCartsServiceAsync();


                foreach (var cart in cartsFromDb)
                {
                    if (cart.Items!.Any())
                    {
                        var progress = await CalculateProgress((double)cart.Items!.Count(i => i.IsChecked), cart.Items!.Count());
                        cart.Progressing = progress;


                    }
                    Carts.Add(new CartDetailsViewModel(cart));

                }
            }
            catch (Exception e)
            {

                OnErrorOccurred(e.Message);
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

        }



        public async Task<Double> CalculateProgress(double checkedCart, int totalCart)
        {
            return checkedCart / totalCart;

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
            try
            {
                Cart cart = new() { Title = TitleName, Items = new List<Item>() };
                await cartService.AddNewCartAsync(cart);
                var shopCartVm = new CartDetailsViewModel(cart);
                Carts.Add(shopCartVm);
                TitleName = string.Empty;
                await OnCloseBottomSheetAsync();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
          
        }

        public async Task OnAddNewItemAsync(FoodData foodData)
        {
          
                var selectedCartUI = Carts.FirstOrDefault(c => c.Id == SelectedCart.Id);
                if (selectedCartUI == null)
                {
                    return;
                }

                var newItem = new Item { ItemName = foodData.Name, IsChecked = false };

                selectedCartUI.Items!.Add(newItem);


                var cart = await cartService.GetCartIdAsync(selectedCartUI.Id);
                cart.Items!.Add(newItem);
                await cartService.UpdateCartAsync(cart);

        }
        private async Task OnUpdateCartAsync()
        {
            try
            {
                var cart = await cartService.GetCartIdAsync(SelectedCart.Id);
                if (cart != null)
                {
                    cart.Title = TitleName!;
                    await cartService.UpdateCartAsync(cart);
                    var cartVm = Carts.FirstOrDefault(c => c.Id == cart.Id);
                    if (cartVm != null)
                    {
                        cartVm.Title = cart.Title;

                    }
                    TitleName = string.Empty;
                    await OnCloseBottomSheetAsync();

                }
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);

            }
           

        }
        private async Task OnDeleteCartCommandAsync()
        {
            var cart = await cartService.GetCartIdAsync(SelectedCart.Id);
            if (cart is not null)
            {
                await cartService.DeleteCartAsync(cart);
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

            TitleName = mode == BottomSheetMode.Add ? string.Empty : SelectedCart.Title ?? string.Empty;
            CurrentBottomSheetMode = mode;
            BottomSheetTitle = mode == BottomSheetMode.Add ? "New" : "Change name";
            var bottomSheet = new CartAddEditBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;


        }
        private async Task OnCloseBottomSheetAsync()
        {
            if (CurrentBottomSheetMode == BottomSheetMode.Add
                || CurrentBottomSheetMode == BottomSheetMode.Edit)
            {
                var buttomSheet = CurrentBottomSheet as CartAddEditBottomSheet;
                buttomSheet!.CartHideKeyboardAsync();
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
            //AppShell.GlobalCartViewModel = this;
            //await Shell.Current.GoToAsync(nameof(ItemPage), true);
            var itemViewModel = new ItemViewModel(new FoodDataService(new FoodDataRepository(new LocalDbService())), this,
                new CartService(new CartRepository(new LocalDbService())));
            await Shell.Current.Navigation.PushAsync(new ItemPage(itemViewModel), true);


            await Task.Run(async () => await itemViewModel.OnInitializeDataAsync());

        }
    }
}
