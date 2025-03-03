using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class ItemPage : ContentPage
{
    public ItemPage(ItemViewModel itemViewModel )
    {
        InitializeComponent();
        itemViewModel.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "Ok");
        BindingContext = itemViewModel;

    }

    
}