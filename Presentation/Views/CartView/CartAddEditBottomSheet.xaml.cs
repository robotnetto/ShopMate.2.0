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

    //private void CustomBottomSheet_Showing(object? sender, EventArgs e)
    //{
    //    if (titleEntry.Focus())
    //    {
    //        titleFrame.BorderColor = Color.FromArgb("#2e80ec");
    //    }
    //}

    private void CartCustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
          CartHideKeyboard();
    }

    //private void KeyboardLoaded()
    //{
    //    titleEntry.Focus();
       
    //    titleEntry.ShowSoftInputAsync(CancellationToken.None);

    //}
    public async void CartHideKeyboard()
    {
        await cartTitleEntry.HideKeyboardAsync(default);
        
    }

}