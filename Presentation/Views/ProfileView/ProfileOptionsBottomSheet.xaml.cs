using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.ProfileView;

public partial class ProfileOptionsBottomSheet : BottomSheet
{
	public ProfileOptionsBottomSheet(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        BindingContext = profileViewModel;
    }

}