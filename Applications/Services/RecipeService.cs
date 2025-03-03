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

        public async Task<Recipe> AddNewRecipeAsync(string recipeName)
        {
           
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!");
            }
            var newRecipe = new Recipe { Title = recipeName, Favorite = false };

            newRecipe.Id = Guid.NewGuid();
            await recipeRepository.CreateAsync(newRecipe);

            return newRecipe;
        }

        public async Task UpdateExistingRecipeAsync(Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!", nameof(recipe.Title));
            }
            await recipeRepository.UpdateAsync(recipe);
        }

        public async Task<Recipe> GetRecipeByIdAsync(Guid id)
        {
            var result = await recipeRepository.GetByIdAsync(id);
            return result ?? throw new NotFoundException("Recipe not found");
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            var result = await recipeRepository.GetAllAsync();
            return result ?? Enumerable.Empty<Recipe>();
        }

        public async Task DeleteExistingRecipeAsync(Recipe recipe)
        {
            await recipeRepository.DeleteAsync(recipe);

        }
    }
}
