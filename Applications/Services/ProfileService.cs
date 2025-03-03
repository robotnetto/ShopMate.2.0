using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Applications.Services
{
    public class ProfileService
    {
        private readonly IProfileRepository profileRepository;

        public ProfileService(IProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        public async Task<Profile> CreateAsync(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
            {
                throw new ArgumentNullException("Profile name cannot be empty");
            }
            var newProfile = new Profile {
                Id = Guid.NewGuid(),
                Name = profileName,
                UniqueCode = UniqueCodeGenerator.GenerateCustomCode(),
                Carts = new List<Cart>() };

            await profileRepository.CreateAsync(newProfile);
            return newProfile;
        }

        public async Task UpdateProfileAsync(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
            {
                throw new ArgumentNullException("Profile name cannot be empty");
            }
            var profile = await profileRepository.GetProfileAsync();
            if (profile == null)
            {
                throw new NotFoundException("Profile not found");
            }
            profile.Name = profileName;
            await profileRepository.UpdateAsync(profile);
        }

        internal async Task<IEnumerable<Profile>> GetAllAsync()
        {
           return await profileRepository.GetAllasync();
        }
        public async Task DeleteProfileAsync()
        {
            var profile = await profileRepository.GetProfileAsync();
            if (profile == null)
            {
                throw new NotFoundException("Profile not found");

            }
            await profileRepository.DeleteAsync(profile);
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await profileRepository.GetProfileAsync();
        }
    }
}
