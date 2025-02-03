using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Interfaces
{
    public interface IFoodDataRepository
    {
        Task CreateAsync(FoodData foodData);
        Task<IEnumerable<FoodData>> GetAllasync();
        Task<FoodData> GetByIdAsync(int id);
        Task UpdateAsync(FoodData foodData);
        Task DeleteAsync(FoodData foodData);
    }
}
