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
        public ObservableCollection<ProfileDetailsViewModel> profiles = new();

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
            foreach (var profile in profiles)
            {
                Profiles.Add(new ProfileDetailsViewModel(profile));
            }
        }
        public async Task AddProfileAsync(string profilename)
        {
            var newProfile = new Profile { Name = profilename, Carts = new List<Cart>()};
            Profiles.Add(new ProfileDetailsViewModel(newProfile));
            await profileService.CreateAsync(newProfile);
        }
    }
}
