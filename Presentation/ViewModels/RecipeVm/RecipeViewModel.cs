using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Services;
using ShopMate._2._0.Presentation.Enum;
using ShopMate._2._0.Presentation.Views.RecipeView;
using System.Text;
using System.Windows.Input;
using The49.Maui.BottomSheet;



namespace ShopMate._2._0.Presentation.ViewModels.RecipeVm
{
    public partial class RecipeViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [ObservableProperty]
        public ObservableRangeCollection<RecipeDetailsViewModel> _recipes = new();

        //private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

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
        public ICommand RecipeSelectCommand { get; }
        public ICommand DebounceRecipeCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand DeleteImageOptionCommand { get; }
        public ICommand DeleteImageCommand { get; }
        public ICommand ShareRecipeCommand { get; }

        private readonly RecipeService _recipeService;
        private readonly ImagePickerService imagePickerService;

        public RecipeViewModel(RecipeService recipeService, ImagePickerService imagePickerService)
        {


            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            this.imagePickerService = imagePickerService;
            AddRecipeCommand = new AsyncRelayCommand(OnAddCommandAsync);
            RemoveRecipeCommand = new AsyncRelayCommand(OnRemoveRecipeAsync);
            UpdateRecipeCommand = new AsyncRelayCommand(OnUpdateCommandAsync);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnOptionsBottomSheetAsync!);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheetAsync);
            SaveRecipeCommand = new AsyncRelayCommand(OnSaveAndEditRecipeAsync);
            RecipeSelectCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnNavigateAsync!);
            DebounceRecipeCommand = new DebounceCommand(SaveRecipeCommand, TimeSpan.FromSeconds(1));
            LoadImageCommand = new AsyncRelayCommand(ImageUploadAsync);
            DeleteImageOptionCommand = new AsyncRelayCommand(OnDeleteImageCommandAsync);
            DeleteImageCommand = new AsyncRelayCommand(OnDeleteImageAsync);
            ShareRecipeCommand = new AsyncRelayCommand(OnShareRecipeAsync);

            InitializeDataAsync();

        }

        public async Task InitializeDataAsync()
        {
            var allRecipes = await _recipeService.GetAllRecipesAsync();
            foreach (var recipe in allRecipes)
            {
                Recipes.Add(new RecipeDetailsViewModel(recipe));
            }
        }

        private async Task OnCloseBottomSheetAsync()
        {
            if (CurrentBottomSheetMode == BottomSheetMode.Add || CurrentBottomSheetMode == BottomSheetMode.Edit)
            {
                var bottomSheet = CurrentBottomSheet as AddEditBottomSheet;

                bottomSheet!.RecipeHideKeyboard();
                await bottomSheet.DismissAsync(true);
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.Options)
            {
                var bottomSheet = CurrentBottomSheet as OptionsBottomSheet;
                await bottomSheet!.DismissAsync(true);
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.DeleteImage)
            {
                var bottomSheet = CurrentBottomSheet as DeleteImageBottomSheet;
                await bottomSheet!.DismissAsync(true);
            }

        }

        private async Task OnNavigateAsync(RecipeDetailsViewModel recipeDetailsViewModel)
        {

            SelectedRecipe = recipeDetailsViewModel;
            CurrentBottomSheetMode = BottomSheetMode.EditDescription;
            await Shell.Current.Navigation.PushAsync(new RecipeDescriptionPage(this));

        }

        private async Task OnOptionsBottomSheetAsync(RecipeDetailsViewModel recipeDetailsViewModel)
        {

            SelectedRecipe = recipeDetailsViewModel;
            var bottomSheet = new OptionsBottomSheet(this);
            await bottomSheet.ShowAsync();
            CurrentBottomSheet = bottomSheet;
            CurrentBottomSheetMode = BottomSheetMode.Options;
            NewRecipeTitle = SelectedRecipe.Title!;


        }

        public async Task ShowBottonSheetAsync(BottomSheetMode mode)
        {

            try
            {
                if (mode == BottomSheetMode.Add || mode == BottomSheetMode.Edit)
                {
                    NewRecipeTitle = mode == BottomSheetMode.Add ? string.Empty : SelectedRecipe?.Title ?? string.Empty;
                    CurrentBottomSheetMode = mode;
                    BottomSheetTitle = mode == BottomSheetMode.Add ? "New" : "Change name";
                    var bottomSheet = new AddEditBottomSheet(this);
                    await bottomSheet.ShowAsync();
                    CurrentBottomSheet = bottomSheet;
                }
                else if (mode == BottomSheetMode.DeleteImage)
                {
                    var bottomSheet = new DeleteImageBottomSheet(this);
                    await bottomSheet.ShowAsync();
                    CurrentBottomSheet = bottomSheet;
                    CurrentBottomSheetMode = BottomSheetMode.DeleteImage;
                }

            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Navigation to CustomBottomSheet failed: {ex.Message}");
            }

        }
        private async Task OnAddCommandAsync()
        {
            await ShowBottonSheetAsync(BottomSheetMode.Add);
        }
        private async Task OnUpdateCommandAsync()
        {
            await CurrentBottomSheet.DismissAsync(true);
            await ShowBottonSheetAsync(BottomSheetMode.Edit);
        }
        private async Task OnDeleteImageCommandAsync()
        {
            await ShowBottonSheetAsync(BottomSheetMode.DeleteImage);

        }

        private async Task OnSaveAndEditRecipeAsync()
        {

            if (CurrentBottomSheetMode == BottomSheetMode.Add)
            {
                await OnSaveNewRecipeAsync();
            }
            else if (CurrentBottomSheetMode == BottomSheetMode.Edit || CurrentBottomSheetMode == BottomSheetMode.EditDescription)
            {
                await OnUpdateRecipeAsync();
            }

        }

        private async Task OnSaveNewRecipeAsync()
        {
            var newRecipe = await _recipeService.AddNewRecipeAsync(NewRecipeTitle);
            Recipes.Add(new RecipeDetailsViewModel(newRecipe));
            NewRecipeTitle = string.Empty;
            await OnCloseBottomSheetAsync();
        }

        private async Task OnUpdateRecipeAsync()
        {
            try
            {
                var selectedRecipe = await _recipeService.GetRecipeByIdAsync(SelectedRecipe.Id);
                if (CurrentBottomSheetMode == BottomSheetMode.Add || CurrentBottomSheetMode == BottomSheetMode.Edit)
                {
                    selectedRecipe.Title = NewRecipeTitle;
                }
                else if (CurrentBottomSheetMode == BottomSheetMode.EditDescription)
                {
                    selectedRecipe.Description = SelectedRecipe.Description;
                }
                selectedRecipe.ImageStream = SelectedRecipe.ImageStream;

                await _recipeService.UpdateExistingRecipeAsync(selectedRecipe);

                //var recipeFromDb = await _recipeService.GetRecipeByIdAsync(SelectedRecipe.Id);
                //SelectedRecipe.Title = recipeFromDb.Title;
                //SelectedRecipe.Description = recipeFromDb.Description;
                //SelectedRecipe.ImageStream = recipeFromDb.ImageStream;

                var existingRecipeIndex = Recipes.IndexOf(Recipes.FirstOrDefault(r => r.Id == selectedRecipe.Id));
                if (existingRecipeIndex != -1)
                {
                    Recipes[existingRecipeIndex] = new RecipeDetailsViewModel(selectedRecipe); 
                }


                await OnCloseBottomSheetAsync();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                await OnCloseBottomSheetAsync();
            }

        }

        private async Task OnRemoveRecipeAsync()
        {

            await CurrentBottomSheet.DismissAsync(true);
            CurrentBottomSheetMode = BottomSheetMode.Remove;
            var selectedRecipe = await _recipeService.GetRecipeByIdAsync(SelectedRecipe.Id);
            await _recipeService.DeleteExistingRecipeAsync(selectedRecipe);
            var recipeVmToRemove = this.Recipes.FirstOrDefault(r => r.Id == SelectedRecipe.Id);
            if (recipeVmToRemove != null)
            {
                Recipes.Remove(recipeVmToRemove!);
            }

        }

        protected virtual void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
        }


        private async Task OnDeleteImageAsync()
        {
            SelectedRecipe.ImageStream = string.Empty;
            await OnUpdateRecipeAsync();
        }
        private async Task ImageUploadAsync()
        {
            SelectedRecipe.ImageStream = await imagePickerService.ImageUploadAsync();
            if (SelectedRecipe.ImageStream != null)
            {
                await OnUpdateRecipeAsync();
               
            }

        }
        private async Task OnShareRecipeAsync()
        {

            var shareItem = new StringBuilder();
            shareItem.AppendLine($"{SelectedRecipe.Title}");
            shareItem.AppendLine("Detail:");
            shareItem.AppendLine(SelectedRecipe.Description);


            await Share.RequestAsync(new ShareTextRequest
            {

                Text = shareItem.ToString()
            });

        }

    }
}
