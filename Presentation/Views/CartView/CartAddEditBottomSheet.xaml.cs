using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using The49.Maui.BottomSheet;



namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartAddEditBottomSheet : BottomSheet
{
    private readonly CartViewModel cartViewModel;

    public CartAddEditBottomSheet(CartViewModel cartViewModel)
    {
        InitializeComponent();
        BindingContext = cartViewModel;

        cartTitleEntry.Focused += (s, e) => titleFrame.BorderColor = Color.FromArgb("#2e80ec");
        Dismissed += CartCustomBottomSheet_Dismissed;
        this.cartViewModel = cartViewModel;
    }



    private void CartCustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
          CartHideKeyboardAsync();
    }

    public async void CartHideKeyboardAsync()
    {
        await cartTitleEntry.HideKeyboardAsync(default);
        
    }

}