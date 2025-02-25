using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;
using ShopMate._2._0.Presentation.Views.ProfileView;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _cartViewModel;
    private readonly ProfileService profileService;

    public CartPage(CartViewModel cartViewModel)
    {
        InitializeComponent();

        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        _cartViewModel = cartViewModel;
        BindingContext = _cartViewModel;
        _cartViewModel.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
        _cartViewModel.DisplayAlert += async (message) => { return await DisplayAlert("Warning", message, "Yes", "Cancel"); };
        _cartViewModel.NavigateTo = NavigateTo;

    }

    private async void NavigateTo()
    {
        await Shell.Current.GoToAsync("//profile");

        //await Navigation.PushAsync(new ProfilePage(profileService));
    }
}