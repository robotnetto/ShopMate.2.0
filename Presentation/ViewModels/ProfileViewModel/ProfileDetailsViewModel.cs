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
        [ObservableProperty]
        private Guid _id;
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _uniqueCode;
        public ProfileDetailsViewModel(Profile profile)
        {
            Id = profile.Id;
            Name = profile.Name!;
            UniqueCode = profile.UniqueCode!;
        }
        public Profile ToProfile()
        {
            return new Profile
            {
                Id = Id,
                Name = Name,
                UniqueCode = UniqueCode
            };  
        }
    }
}
