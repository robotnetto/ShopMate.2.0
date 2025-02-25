using Microsoft.EntityFrameworkCore;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly LocalDbService localDbService;

        public ProfileRepository(LocalDbService localDbService)
        {
            this.localDbService = localDbService;
        }
        public async Task CreateAsync(Profile profile)
        {
            await localDbService.Profiles.AddAsync(profile);
            await localDbService.SaveChangesAsync();
        }

        public async Task DeleteAsync(Profile profile)
        {
             localDbService.Profiles.Remove(profile);
            await localDbService.SaveChangesAsync();
        }

        public async Task<IEnumerable<Profile>> GetAllasync()
        {
            return await localDbService.Profiles.ToListAsync();
        }

        public async Task<Profile> GetByIdAsync(int id)
        {
            return await localDbService.Profiles.FindAsync(id);

        }
        public async Task<Profile> GetProfileAsync()
        {
            return await localDbService.Profiles.FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Profile profile)
        {
            localDbService.Profiles.Update(profile);
            await localDbService.SaveChangesAsync();
        }
    }
}
