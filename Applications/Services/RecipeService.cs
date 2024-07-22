using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;

namespace ShopMate._2._0.Applications.Services
{
    public class RecipeService
    {
        private readonly IRecipeRepository recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            this.recipeRepository = recipeRepository;
        }

        public async Task AddNewRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe), "Recipe parameter cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Title canot empty or whitespace!", nameof(recipe.Title));
            }

            recipe.Id = Guid.NewGuid();
            await recipeRepository.CreateAsync(recipe);

        }

        public async Task UpdateRecipe(Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Title canot empty or whitespace!", nameof(recipe.Title));
            }
            await recipeRepository.UpdateAsync(recipe);
        }

        public async Task<Recipe> GetRecipeId(Guid id)
        {
            var result = await recipeRepository.GetByIdAsync(id);
            return result ?? throw new NotFoundException("Recipe not found");
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipies()
        {
            var result = await recipeRepository.GetAllAsync();
            return result ?? Enumerable.Empty<Recipe>();
        }

        public async Task DeleteRecipe(Recipe recipe)
        {
            await recipeRepository.DeleteAsync(recipe);

        }
    }
}
