using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;

namespace ShopMate._2._0.Presentation.Views.ProfileView;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel profileViewModel;
	public ProfilePage(ProfileService profileService)
	{
		InitializeComponent();
		 profileViewModel  = new ProfileViewModel(profileService);
		BindingContext = profileViewModel;

    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		var profileName = await DisplayPromptAsync("Profile Name", "Enter your profile name", "Save", "Cancel", string.Empty , 16, Keyboard.Default, string.Empty);
        if (!string.IsNullOrWhiteSpace(profileName))
		{
			await profileViewModel.AddProfileAsync(profileName);
        }
    }
}