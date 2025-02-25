using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using System.Collections.ObjectModel;


namespace ShopMate._2._0.Presentation.ViewModels.ProfileViewModel
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly ProfileService profileService;
        [ObservableProperty]
        public ProfileDetailsViewModel _profile;
        //[ObservableProperty]
        //public string profileName;
        //public ICommand AddProfileCommand { get; }

        public ProfileViewModel(ProfileService profileService)
        {
            this.profileService = profileService;
            //AddProfileCommand = new AsyncRelayCommand(AddProfileAsync);
            InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            var profiles = await profileService.GetAllAsync();

            if (profiles.Any())
            {
                Profile = new ProfileDetailsViewModel(profiles.First());
            }

        }
        public async Task AddProfileAsync(string profilename)
        {
            var newProfile = new Profile { Name = profilename, Carts = new List<Cart>()};
            Profile = new ProfileDetailsViewModel(newProfile);
            await profileService.CreateAsync(newProfile);
        }

        public async Task UpdateProfileAsync(string profileName)
        {
            Profile.Name = profileName;   
            await profileService.UpdateProfileAsync(Profile.ToProfile());
        }
    }
}
