using CommunityToolkit.Maui.Alerts;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class ItemPage : ContentPage
{
    private readonly ItemViewModel itemViewModel;
    public ItemPage(ItemViewModel itemViewModel )
    {
        InitializeComponent();
        
        //this.itemViewModel = itemViewModel;
        BindingContext = itemViewModel;

       
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Reload data when the page appears again
        Task.Run(async () => await itemViewModel.OnInitializeDataAsync());
    }
    //private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    //{
    //    if (isInitializing)
    //        return;
    //    if (sender is CheckBox checkBox && checkBox.BindingContext is Item item)
    //    {
    //        // Prevent infinite loop by checking if the value actually changed
    //        if (item.IsChecked == e.Value) return;

    //        item.IsChecked = e.Value;
    //        await itemViewModel.OnCheckItemCommand(item);
    //    }
    //}
    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await Task.Delay(350);
    //    _ = Task.Run(async () => await itemViewModel.OnInitializeDataAsync());
    //    isInitializing = false;
    //}

    //private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs  e )
    //{
    //    OnPropertyChanged(nameof(cartViewModel));

    //}
}