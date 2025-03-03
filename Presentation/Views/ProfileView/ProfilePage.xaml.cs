using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;

namespace ShopMate._2._0.Presentation.Views.ProfileView;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel profileViewModel;
    private readonly ProfileService profileService;

    public ProfilePage(ProfileService profileService, CartViewModel cartViewModel)
	{
		InitializeComponent();
		profileViewModel = new ProfileViewModel(profileService, cartViewModel);
		BindingContext = profileViewModel;
		profileViewModel.ConfirmDelete = ConfirmDelete;
        this.profileService = profileService;
    }

	public async Task<bool> ConfirmDelete()
	{
		return await DisplayAlert("Delete Profile", "Are you sure you want to delete this profile \nIncluding carts and items in cart will be deleted?", "Yes", "No");

	}
}