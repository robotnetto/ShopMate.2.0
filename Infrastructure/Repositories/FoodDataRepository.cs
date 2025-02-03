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
    public class FoodDataRepository : IFoodDataRepository
    {
        private readonly LocalDbService localDbService;

        public FoodDataRepository(LocalDbService localDbService )
        {
            this.localDbService = localDbService;
        }
        public async Task CreateAsync(FoodData foodData)
        {
            if (foodData == null)
            {
                throw new ArgumentNullException(nameof(foodData));
            }
           await localDbService.FoodItems.AddAsync(foodData);
            await localDbService.SaveChangesAsync();

        }

        public Task DeleteAsync(FoodData foodData)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FoodData>> GetAllasync()
        {
            var result = await localDbService.FoodItems.ToListAsync();
            return result;
        }

        public Task<FoodData> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(FoodData foodData)
        {
            throw new NotImplementedException();
        }
    }
}
