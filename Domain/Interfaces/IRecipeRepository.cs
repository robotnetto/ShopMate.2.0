using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Domain.Interfaces
{
    public interface IRecipeRepository
    {
        Task CreateAsync(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllAsync();
        Task<Recipe> GetByIdAsync(Guid id);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(Recipe recipe);
    }
}
