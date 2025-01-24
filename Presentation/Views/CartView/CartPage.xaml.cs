using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Presentation.ViewModels.CartVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartPage : ContentPage
{
    public CartPage()
    {
        InitializeComponent();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        var cartViewModel = new CartViewModel();
        cartViewModel.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
        BindingContext = cartViewModel;
    }
}