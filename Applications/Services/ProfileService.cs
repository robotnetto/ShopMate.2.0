using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Interfaces;
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

        public async Task CreateAsync(Profile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                throw new ArgumentNullException("Profile name cannot be empty");
            }
           profile.Id = Guid.NewGuid();
            await profileRepository.CreateAsync(profile);
        }

        public async Task UpdateProfileAsync(Profile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                throw new ArgumentNullException("Profile name cannot be empty");
            }
            await profileRepository.UpdateAsync(profile);
        }

        internal async Task<IEnumerable<Profile>> GetAllAsync()
        {
            return await profileRepository.GetAllasync();
        }
    }
}
