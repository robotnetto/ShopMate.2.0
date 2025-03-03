using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.ProfileView;

public partial class AddEditProfileBottomSheet : BottomSheet
{
    public AddEditProfileBottomSheet(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        BindingContext = profileViewModel;
        profileEntry.Focused += (s, e) => profileFrame.BorderColor = Color.FromArgb("#2e80ec");
        Dismissed  += HideKeyboardOnDismissAsync;
    }

    private void HideKeyboardOnDismissAsync(object? sender, DismissOrigin e)
    {
        HideKeyboardOnDismissAsync();
    }
    public async void HideKeyboardOnDismissAsync()
    {
       await profileEntry.HideKeyboardAsync(default);
    }
}