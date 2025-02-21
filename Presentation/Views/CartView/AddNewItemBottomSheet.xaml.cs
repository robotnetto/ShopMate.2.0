using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class AddNewItemBottomSheet : BottomSheet
{
    

    private readonly ItemViewModel itemViewModel;
    public AddNewItemBottomSheet(ItemViewModel itemViewModel)
	{
		InitializeComponent();
        BindingContext = itemViewModel;
        searchItemEntry.Focused += (s, e) => searchFrame.BorderColor = Color.FromArgb("#2e80ec");
        Dismissed += CustomBottomSheet_Dismissed;
        CalculateHeight();
        this.itemViewModel = itemViewModel;
        
    }
   

    private void CustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
        HideKeyboardOnDismissAsync();
        itemViewModel.SearchText = string.Empty;
    }
    internal async void HideKeyboardOnDismissAsync()
    {
        await searchItemEntry.HideKeyboardAsync(default);
    }
    internal async void ShowKeyboardOnAppearAsync()
    {
        await searchItemEntry.ShowKeyboardAsync(default);
    }
    private async void TapGestureRecognizer_Tapped(object sender, EventArgs args) 
    {
        if (searchItemEntry != null && searchItemEntry.IsFocused)
        {
            searchItemEntry.Unfocus();
            HideKeyboardOnDismissAsync();
        }
    }
    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (itemViewModel != null && itemViewModel.DebounceSearchItemsCommand.CanExecute(null))
        {
            itemViewModel.DebounceSearchItemsCommand.Execute(null);
        }
    }

    private void CalculateHeight()
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        var bottomSheetHeight = screenHeight * 0.95;
        this.Detents.Add(new HeightDetent { Height = bottomSheetHeight});
    }

    private void ItemsCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.VerticalOffset != 0)
        {   
            searchFrame.BorderColor = Color.FromArgb("#FFFFFF");
            searchItemEntry.Unfocus();
            HideKeyboardOnDismissAsync();

        }

    }
    //public async Task ShowSnackbarAsync()
    //{
    //    var snackbarOptions = new SnackbarOptions
    //    {
    //        BackgroundColor = Color.FromArgb("#39de57"),
    //        TextColor = Color.FromArgb("#FFFFFF"),
    //        CornerRadius = 10,

    //    };
       
    //    var snackbar = Snackbar.Make($"BottomSheet", () => { }, string.Empty, TimeSpan.FromSeconds(2), snackbarOptions);
    //    await this.Dispatcher.DispatchAsync(async () => await snackbar.Show());
    //}

}