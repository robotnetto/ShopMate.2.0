using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.Views.ProfileView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The49.Maui.BottomSheet;


namespace ShopMate._2._0.Presentation.ViewModels.ProfileViewModel
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly ProfileService profileService;
        private readonly CartViewModel cartViewModel;
        [ObservableProperty]
        public ProfileDetailsViewModel _profile;
      
        private BottomSheet currentBS;
        [ObservableProperty]
        public string _bottomSheetTitle;
        [ObservableProperty]
        public string _profileName;
        public Func<Task<bool>> ConfirmDelete { get; set; }
        public ICommand AddProfileCommand { get; }
        public ICommand EditProfiletCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand CloseCommand { get; }
        public ProfileViewModel(ProfileService profileService, CartViewModel cartViewModel)
        {
            this.profileService = profileService;
            this.cartViewModel = cartViewModel;
            AddProfileCommand = new AsyncRelayCommand(NewProfileCommand);
            DeleteProfileCommand = new AsyncRelayCommand(OnDeleteProfileCommand);
            EditProfiletCommand = new AsyncRelayCommand(OnEditProfileCommand);
            SaveProfileCommand = new AsyncRelayCommand(OnSaveProfileCommand);
            CloseCommand = new RelayCommand(OnCloseCommand);
            _= InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            var profiles = await profileService.GetProfileAsync();

            if (profiles != null)
            {
                Profile = new ProfileDetailsViewModel(profiles);
            }
           
        }
        private async Task NewProfileCommand()
        {
            if (Profile is null)
            {
                BottomSheetTitle = "New Profile";
                ProfileName = string.Empty;
                var bottomSheet = new AddEditProfileBottomSheet(this);
                currentBS = bottomSheet;
                await bottomSheet.ShowAsync();
            }

        }
        private async Task OnEditProfileCommand()
        {
            if (Profile == null) return;

            BottomSheetTitle = "Edit Profile";
            ProfileName = Profile.Name;
            var bottomSheet = new AddEditProfileBottomSheet(this);
            currentBS = bottomSheet;
            await bottomSheet.ShowAsync();

        }

        public async Task OnSaveProfileCommand()
        {
            if (Profile != null && !string.IsNullOrWhiteSpace(ProfileName))
            {
                await UpdateProfileAsync(ProfileName);
            }
            else
            {
                await AddProfileAsync();
            }
            OnCloseCommand();
        }

        private async void OnCloseCommand()
        {
            var bottomSheet = currentBS as AddEditProfileBottomSheet;
            if (bottomSheet != null)
            {
                bottomSheet.HideKeyboardOnDismissAsync();
                await bottomSheet.DismissAsync();
                ProfileName = string.Empty;
            }
        }
        private async Task OnDeleteProfileCommand()
        {
            if (Profile == null) return;
            if (await ConfirmDelete())
            {
                await profileService.DeleteProfileAsync();
                Profile = null;
                cartViewModel.Carts = null;
            }
               
        }

        private async Task AddProfileAsync()
        {
            if (!string.IsNullOrWhiteSpace(ProfileName))
            {
               var profile = await profileService.CreateAsync(ProfileName);
                Profile = new ProfileDetailsViewModel(profile);
            }
        }

        public async Task UpdateProfileAsync(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName)) return;
            Profile.Name = profileName;
            await profileService.UpdateProfileAsync(profileName);
        }
    }
}
