using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.ProfileViewModel
{
    public partial class ProfileDetailsViewModel : ObservableObject
    {
        private readonly Profile _profile;
        [ObservableProperty]
        private Guid _id;
        [ObservableProperty]
        private string _name;
        public ProfileDetailsViewModel(Profile profile)
        {
            Id = profile.Id;
            Name = profile.Name;
        }
        public Profile ToProfile()
        {
            _profile.Name = Name;
            return _profile;
        }
    }
}
