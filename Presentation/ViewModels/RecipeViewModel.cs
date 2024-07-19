using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.Views.RecipeView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The49.Maui.BottomSheet;
using Xamarin.Essentials;


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

        [ObservableProperty]
        private string _newRecipeTitle = string.Empty;
        private readonly RecipeService _recipeService;

        private BottomSheet _currentBottomSheet;
        public BottomSheet CurrentBottomShhet
        {
            get => _currentBottomSheet;
            set
            {
                if (_currentBottomSheet != value)
                {
                    _currentBottomSheet = value;
                    OnPropertyChanged(nameof(CurrentBottomShhet));
                }
            }
        }
        public ICommand AddRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UpdateRecipeCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand CloseCommand { get; }
        public RecipeViewModel(RecipeService recipeService)
        {
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            AddRecipeCommand = new AsyncRelayCommand(OnAddBotomSheet);
            RemoveRecipeCommand = new AsyncRelayCommand(OnRemoveRecipe);
            UpdateRecipeCommand = new AsyncRelayCommand(OnUpdateRecipe);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnOptions!);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheet);
            SaveRecipeCommand = new AsyncRelayCommand(OnAddRecipe);
            _ = InitializeDataAsync();
        }

        private async Task OnCloseBottomSheet()
        {
            if (CurrentBottomShhet is AddBottomSheet addBottomSheet)
            {
                await addBottomSheet.HideKeyboard();
            }

            await CurrentBottomShhet.DismissAsync(true);

        }

        private async Task OnOptions(RecipeDetailsViewModel recipeDetailsViewModel)
        {
            try
            {
                SelectedRecipe = recipeDetailsViewModel;
                var bottomSheet = new OptionsBottomSheet(this);
                await bottomSheet.ShowAsync();
                CurrentBottomShhet = bottomSheet;
                //await Shell.Current.GoToAsync(nameof(bottomSheet));
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Navigation to CustomBottomSheet failed: {ex.Message}");
            }
        }


        public RecipeViewModel() : this(new RecipeService(new RecipeRepository(new LocalDbService())))
        {
        }

        private async Task InitializeDataAsync()
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
        private async Task OnAddBotomSheet()
        {
            var bottomSheet = new AddBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomShhet = bottomSheet;

        }

        private async Task OnAddRecipe()
        {
            try
            {
                Recipe newRecipe = new() { Title = NewRecipeTitle, Favorite = false };
                await _recipeService.AddNewRecipe(newRecipe);
                var recipeVm = new RecipeDetailsViewModel(newRecipe);
                Recipes.Add(recipeVm);
                NewRecipeTitle = string.Empty;
                await OnCloseBottomSheet();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }

        }

        private async Task OnUpdateRecipe()
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

        private async Task OnRemoveRecipe()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                await _recipeService.DeleteRecipe(selectedRecipe);
                var recipeVmToRemove = this.Recipes.FirstOrDefault(r => r.Id == SelectedRecipe.Id);
                if (recipeVmToRemove != null)
                {
                    Recipes.Remove(recipeVmToRemove!);
                }
                await OnCloseBottomSheet();
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
