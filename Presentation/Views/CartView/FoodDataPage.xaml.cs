using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class FoodDataPage : ContentPage
{
    private readonly ItemViewModel itemViewModel;

    public FoodDataPage(ItemViewModel itemViewModel)
    {
        InitializeComponent();
        this.itemViewModel = itemViewModel;
        BindingContext = itemViewModel;
        searchItemEntry.Focused += (s, e) => searchFrame.BorderColor = Color.FromArgb("#2e80ec");
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs args)
    {
        if (searchItemEntry != null && searchItemEntry.IsFocused)
        {
            searchItemEntry.Unfocus();
        }
    }
    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (itemViewModel != null && itemViewModel.DebounceSearchItemsCommand.CanExecute(null))
        {
            itemViewModel.DebounceSearchItemsCommand.Execute(null);
        }
    }
    private void ItemsCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.VerticalDelta != 0)
        {
            searchFrame.BorderColor = Color.FromArgb("#FFFFFF");
            searchItemEntry.Unfocus();
             HideKeyboardOnDismissAsync();
        }

    }
   
    internal async void HideKeyboardOnDismissAsync()
    {
        await searchItemEntry.HideKeyboardAsync(default);
    }
   
}