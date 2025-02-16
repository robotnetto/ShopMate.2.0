using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _cartViewModel;
    public CartPage()
    {
        InitializeComponent();
      
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        _cartViewModel = new CartViewModel();
        BindingContext = _cartViewModel;
        _cartViewModel.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
    }
   
}