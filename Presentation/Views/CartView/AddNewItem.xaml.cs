using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class AddNewItem : BottomSheet
{
    
    public AddNewItem(CartViewModel cartViewModel)
	{
		InitializeComponent();
        BindingContext = cartViewModel;
        itemTitleEntry.Focused += (s, e) => titleFrame.BorderColor = Color.FromArgb("#2e80ec");
        Dismissed += CartCustomBottomSheet_Dismissed;
        CalculateHeight();
    }
    private void CartCustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
        HideKeyboardOnDismissAsync();
    }
    internal async void HideKeyboardOnDismissAsync()
    {
       await itemTitleEntry.HideKeyboardAsync(default);
    }
    internal async void ShowKeyboardOnAppearAsync()
    {
        await itemTitleEntry.ShowKeyboardAsync(default);
    }
    private void CalculateHeight()
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        var bottomSheetHeight = screenHeight * 0.95;
        this.Detents.Add(new HeightDetent { Height = bottomSheetHeight});
    }
}