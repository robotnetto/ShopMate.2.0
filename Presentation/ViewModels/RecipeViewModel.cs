using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ShopMate._2._0.Presentation.ViewModels
{
    public partial class RecipeViewModel : ObservableObject
    {
        public ObservableCollection<RecipeDetailsViewModel> _recipes = new();
        public ObservableCollection<RecipeDetailsViewModel> Recipes
        {
            get => _recipes;
            set
            {
                if (_recipes != value)
                {
                    _recipes = value;
                    OnPropertyChanged(nameof(Recipes));
                }
            }
        }

        [ObservableProperty]
        private RecipeDetailsViewModel _selectedRecipe;

        public event Action<string>? ErrorOccurred;

        //[ObservableProperty]
        //private string _newRecipeTitle = string.Empty;
        private readonly RecipeService _recipeService;

        public ICommand AddRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UpdateRecipeCommand { get; }
        public ICommand OptionsCommand { get; }

        public RecipeViewModel(RecipeService recipeService)
        {
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            AddRecipeCommand = new AsyncRelayCommand(AddRecipe);
            RemoveRecipeCommand = new AsyncRelayCommand(RemoveRecipe);
            UpdateRecipeCommand = new AsyncRelayCommand(UpdateRecipe);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(Options);
            _ = InitializedDataAsync();
        }

        private async Task Options(RecipeDetailsViewModel recipeDetailsView)
        {
            SelectedRecipe = recipeDetailsView;
           
            await Shell.Current.GoToAsync(nameof(BottomSheet));
        } 
       

        public RecipeViewModel() : this(new RecipeService( new RecipeRepository(new LocalDbService())))
        {
        }

        private async Task InitializedDataAsync()
        {
            var allRecipes = await _recipeService.GetAllRecipies();
            foreach (var recipe in allRecipes)
            {
                var recipeVm = new RecipeDetailsViewModel(recipe);
                Recipes.Add(recipeVm);
            }

        }

        //private async Task OnAddRecipeCommand()
        //{
        //    string result = await Shell.Current.DisplayPromptAsync("New recipe", "Title name", "OK" ,"Cancel");
        //    //await Shell.Current.GoToAsync(nameof(AddRecipePage));
        //}
        private async Task AddRecipe()
        {
            try
            {
                string titleName = await Shell.Current.DisplayPromptAsync("New recipe", "Title name");
                if (string.IsNullOrEmpty(titleName))
                {
                    return;
                }

                Recipe newRecipe = new() { Title = titleName, Favorite = false };
                await _recipeService.AddNewRecipe(newRecipe);
                var recipeVm = new RecipeDetailsViewModel(newRecipe);
                Recipes.Add(recipeVm);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
        }

        private async Task UpdateRecipe()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                selectedRecipe.Title = SelectedRecipe.Title;
                selectedRecipe.Favorite = SelectedRecipe.Favorite;
                selectedRecipe.Description = SelectedRecipe.Description;

                await _recipeService.UpdateRecipe(selectedRecipe);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }

        }

        private async Task RemoveRecipe()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                await _recipeService.DeleteRecipe(selectedRecipe);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
        }

        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }

    }
}
