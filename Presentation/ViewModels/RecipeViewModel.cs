using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.Enum;
using ShopMate._2._0.Presentation.Views.RecipeView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The49.Maui.BottomSheet;


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
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        [ObservableProperty]
        private RecipeDetailsViewModel _selectedRecipe;

        [ObservableProperty]
        private string _bottomSheetTitle;
        public event Action<string>? ErrorOccurred;

        [ObservableProperty]
        private string _newRecipeTitle = string.Empty;
        [ObservableProperty]
        private string _newRecipeDescription = string.Empty;

        private readonly RecipeService _recipeService;

        private BottomSheet _currentBottomSheet;
        public BottomSheet CurrentBottomSheet
        {
            get => _currentBottomSheet;
            set
            {
                if (_currentBottomSheet != value)
                {
                    _currentBottomSheet = value;
                    OnPropertyChanged(nameof(CurrentBottomSheet));
                }
            }
        }
        public ICommand AddRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UpdateRecipeCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CardSelectedCommand { get; }
        public RecipeViewModel(RecipeService recipeService)
        {
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            AddRecipeCommand = new AsyncRelayCommand(OnAddBotomSheet);
            RemoveRecipeCommand = new AsyncRelayCommand(OnRemoveRecipe);
            UpdateRecipeCommand = new AsyncRelayCommand(OnUpdateBottomSheet);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnOptionsBottomSheet!);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheet);
            SaveRecipeCommand = new AsyncRelayCommand(OnAddAndEditRecipe);
            CardSelectedCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnNavigate!);
            _ = InitializeDataAsync();
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

        private async Task OnCloseBottomSheet()
        {

            if (CurrentBottomSheet is AddBottomSheet addBottomSheet)
            {
                await addBottomSheet.HideKeyboard();
                await addBottomSheet.DismissAsync(true);
            }
            else if (CurrentBottomSheet is EditBottomSheet editBottomSheet)
            {
                await editBottomSheet.HideKeyboard();
                await editBottomSheet.DismissAsync(true);
            }
           await CurrentBottomSheet.DismissAsync(true);
        }

        private async Task OnUpdateBottomSheet()
        {
            await CurrentBottomSheet.DismissAsync(true);
            var bottomSheet = new EditBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;

        }
        private async Task OnNavigate(RecipeDetailsViewModel recipeDetailsViewModel)
        {
            SelectedRecipe = recipeDetailsViewModel;

            await Shell.Current.Navigation.PushAsync(new RecipeDescription(this));

        }

        private async Task OnOptionsBottomSheet(RecipeDetailsViewModel recipeDetailsViewModel)
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                SelectedRecipe = recipeDetailsViewModel;
                var bottomSheet = new OptionsBottomSheet(this);
                await bottomSheet.ShowAsync();
                CurrentBottomSheet = bottomSheet;
                BottomSheetTitle = BottomSheetMode.Edit.ToString();
                NewRecipeTitle = SelectedRecipe.Title!;
                //await Shell.Current.GoToAsync(nameof(bottomSheet));
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Navigation to CustomBottomSheet failed: {ex.Message}");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }


        //private async Task OnAddRecipeCommand()
        //{
        //    string result = await Shell.Current.DisplayPromptAsync("New recipe", "Title name", "OK" ,"Cancel");
        //    //await Shell.Current.GoToAsync(nameof(AddRecipePage));
        //}
        private async Task OnAddBotomSheet()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                NewRecipeTitle = string.Empty;
                BottomSheetTitle = BottomSheetMode.Add.ToString();
                var bottomSheet = new AddBottomSheet(this);
                await bottomSheet.ShowAsync();
                CurrentBottomSheet = bottomSheet;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Navigation to CustomBottomSheet failed: {ex.Message}");

            }
            finally
            {
                semaphoreSlim.Release();
            }

        }

        private async Task OnAddAndEditRecipe()
        {
            try
            {
                if ( CurrentBottomSheet is AddBottomSheet )
                {

                    await OnAddNewRecipe();
                }
                else

                await OnUpdateRecipe();

            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }

        }

        private async Task OnAddNewRecipe()
        {

            Recipe newRecipe = new() { Title = NewRecipeTitle, Favorite = false };
            await _recipeService.AddNewRecipe(newRecipe);
            var recipeVm = new RecipeDetailsViewModel(newRecipe);
            Recipes.Add(recipeVm);
            NewRecipeTitle = string.Empty;
            await OnCloseBottomSheet();
        }

        private async Task OnUpdateRecipe()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                if (CurrentBottomSheet is EditBottomSheet || CurrentBottomSheet is AddBottomSheet)
                {
                    selectedRecipe.Title = NewRecipeTitle;
                    selectedRecipe.Favorite = SelectedRecipe.Favorite;
                }

                selectedRecipe.Description = NewRecipeDescription;


                await _recipeService.UpdateRecipe(selectedRecipe);
                var updatedRecipe = await _recipeService.GetRecipeId(selectedRecipe.Id);

                SelectedRecipe.Title = updatedRecipe.Title;
                SelectedRecipe.Favorite = updatedRecipe.Favorite;
                SelectedRecipe.Description = updatedRecipe.Description;
                await OnCloseBottomSheet();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                await OnCloseBottomSheet();
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
