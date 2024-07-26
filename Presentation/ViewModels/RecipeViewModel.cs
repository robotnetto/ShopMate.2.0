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
        [ObservableProperty]
        public ObservableCollection<RecipeDetailsViewModel> _recipes = new();

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

        private BottomSheetMode CurrentBottomSheetMode { get; set; }
        private BottomSheet CurrentBottomSheet { get; set; }
        public ICommand AddRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UpdateRecipeCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CardSelectedCommand { get; }
        public ICommand DebounceRecipeCommand { get; }

        private readonly RecipeService _recipeService;
        public RecipeViewModel(RecipeService recipeService)
        {


            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            AddRecipeCommand = new AsyncRelayCommand(OnAddBottomSheet);
            RemoveRecipeCommand = new AsyncRelayCommand(OnRemoveRecipe);
            UpdateRecipeCommand = new AsyncRelayCommand(OnUpdateBottomSheet);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnOptionsBottomSheet!);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheet);
            SaveRecipeCommand = new AsyncRelayCommand(OnAddAndEditRecipe);
            CardSelectedCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnNavigate!);
            DebounceRecipeCommand = new DebounceCommand(SaveRecipeCommand, TimeSpan.FromSeconds(1));

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
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }

            try
            {
                if (CurrentBottomSheetMode == BottomSheetMode.Add || CurrentBottomSheetMode == BottomSheetMode.Edit)
                {
                    var bottomSheet = CurrentBottomSheet as AddEditBottomSheet;
                    await bottomSheet!.HideKeyboard();
                    await bottomSheet.DismissAsync(true);
                }
                else if (CurrentBottomSheetMode == BottomSheetMode.Remove)
                {
                    var bottomSheet = CurrentBottomSheet as OptionsBottomSheet;
                    await bottomSheet!.DismissAsync(true);
                }

            }
            finally
            {
                semaphoreSlim.Release();
            }

        }

        private async Task OnNavigate(RecipeDetailsViewModel recipeDetailsViewModel)
        {
            SelectedRecipe = recipeDetailsViewModel;
            CurrentBottomSheetMode = BottomSheetMode.EditDescription;
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

        public async Task ShowBottonSheet(BottomSheetMode mode)
        {

            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                NewRecipeTitle = mode == BottomSheetMode.Add ? string.Empty : SelectedRecipe?.Title ?? string.Empty;
                CurrentBottomSheetMode = mode;
                BottomSheetTitle = mode == BottomSheetMode.Add ? "Add Title" : "Edit Title";
                var bottomSheet = new AddEditBottomSheet(this);
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
        private async Task OnAddBottomSheet()
        {
            await ShowBottonSheet(BottomSheetMode.Add);
        }
        private async Task OnUpdateBottomSheet()
        {
            await CurrentBottomSheet.DismissAsync(true);
            await ShowBottonSheet(BottomSheetMode.Edit);
        }

        private async Task OnAddAndEditRecipe()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                if (CurrentBottomSheetMode == BottomSheetMode.Add)
                {
                    await OnAddNewRecipe();
                }
                else if (CurrentBottomSheetMode == BottomSheetMode.Edit || CurrentBottomSheetMode == BottomSheetMode.EditDescription)
                {
                    await OnUpdateRecipe();
                }


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
            semaphoreSlim.Release();
            await OnCloseBottomSheet();
        }

        private async Task OnUpdateRecipe()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                if (CurrentBottomSheetMode == BottomSheetMode.Add || CurrentBottomSheetMode == BottomSheetMode.Edit)
                {
                    selectedRecipe.Title = NewRecipeTitle;
                }
                else if (CurrentBottomSheetMode == BottomSheetMode.EditDescription)
                {
                    selectedRecipe.Description = SelectedRecipe.Description;
                }
                selectedRecipe.Favorite = SelectedRecipe.Favorite;
               


                await _recipeService.UpdateRecipe(selectedRecipe);
                var updatedRecipe = await _recipeService.GetRecipeId(selectedRecipe.Id);

                SelectedRecipe.Title = updatedRecipe.Title;
                SelectedRecipe.Favorite = updatedRecipe.Favorite;
                SelectedRecipe.Description = updatedRecipe.Description;
                semaphoreSlim.Release();
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
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                CurrentBottomSheetMode = BottomSheetMode.Remove;
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                await _recipeService.DeleteRecipe(selectedRecipe);
                var recipeVmToRemove = this.Recipes.FirstOrDefault(r => r.Id == SelectedRecipe.Id);
                if (recipeVmToRemove != null)
                {
                    Recipes.Remove(recipeVmToRemove!);
                }
                semaphoreSlim.Release();
                await CurrentBottomSheet.DismissAsync(true);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
            //finally
            //{
            //    semaphoreSlim.Release();
            //}

        }

        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }


    }
}
