using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Interfaces
{
    public interface IProfileRepository
    {
        Task CreateAsync(Profile profile);
        Task<IEnumerable<Profile>> GetAllasync();
        Task<Profile> GetByIdAsync(int id);
        Task UpdateAsync(Profile profile);
        Task DeleteAsync(Profile profile);
        Task<Profile> GetProfileAsync();

    }
}
