using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.Enum;
using ShopMate._2._0.Presentation.Views.RecipeView;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The49.Maui.BottomSheet;


namespace ShopMate._2._0.Presentation.ViewModels.RecipeVm
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
        public ICommand LoadImageCommand { get; }

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
            LoadImageCommand = new AsyncRelayCommand(ImageUpload);

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
                     bottomSheet!.HideKeyboard();
                    await bottomSheet.DismissAsync(true);
                }
                else if (CurrentBottomSheetMode == BottomSheetMode.Options)
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
                CurrentBottomSheetMode = BottomSheetMode.Options;
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
                selectedRecipe.ImageStream = SelectedRecipe.ImageStream;

                await _recipeService.UpdateRecipe(selectedRecipe);

                var updatedRecipe = await _recipeService.GetRecipeId(selectedRecipe.Id);

                SelectedRecipe.Title = updatedRecipe.Title;
                SelectedRecipe.Favorite = updatedRecipe.Favorite;
                SelectedRecipe.Description = updatedRecipe.Description;
                SelectedRecipe.ImageStream = updatedRecipe.ImageStream;
            }
            catch (Exception e)
            {
                semaphoreSlim.Release();
                OnErrorOccurred(e.Message);
                await OnCloseBottomSheet();
            }
            finally { semaphoreSlim?.Release(); }
            await OnCloseBottomSheet();

        }

        private async Task OnRemoveRecipe()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
            {
                await CurrentBottomSheet.DismissAsync(true);
                CurrentBottomSheetMode = BottomSheetMode.Remove;
                var selectedRecipe = await _recipeService.GetRecipeId(SelectedRecipe.Id);
                await _recipeService.DeleteRecipe(selectedRecipe);
                var recipeVmToRemove = this.Recipes.FirstOrDefault(r => r.Id == SelectedRecipe.Id);
                if (recipeVmToRemove != null)
                {
                    Recipes.Remove(recipeVmToRemove!);
                }
                semaphoreSlim.Release();
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

        private async Task ImageUpload()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            var image = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select a photo" });
            if (image == null)
            {
                semaphoreSlim.Release();
                return;
            }
            if (image != null)
            {
                try
                {
                    var stream = await image.OpenReadAsync();
                    byte[] result;
                    using (var streamReader = new MemoryStream())
                    {
                        stream.CopyTo(streamReader);
                        result = streamReader.ToArray();
                    }
                    // Resize the image using SkiaSharp
                    byte[] resizedImage = ResizeImage(result, 530, 310); // Resize to 800x800

                    var imagePath = Convert.ToBase64String(resizedImage);
                    imagePath = string.Format("data:image/png;base64,{0}", imagePath);

                    SelectedRecipe.ImageStream = imagePath;

                }
                catch (Exception ex)
                {
                    // Handle exception
                }
                await OnUpdateRecipe();
            }
        }
        private byte[] ResizeImage(byte[] imageData, int width, int height)
        {
            using (var inputStream = new MemoryStream(imageData))
            {
                using (var original = SKBitmap.Decode(inputStream))
                {
                    float aspectRatio = Math.Min((float)width / original.Width, (float)height / original.Height);

                    int newWidth = (int)(original.Width * aspectRatio);
                    int newHeight = (int)(original.Height * aspectRatio);

                    var info = new SKImageInfo(newWidth, newHeight);
                    using (var resized = original.Resize(info, SKFilterQuality.Medium))
                    {
                        if (resized == null)
                            return imageData;

                        using (var image = SKImage.FromBitmap(resized))
                        {
                            using (var outputStream = new MemoryStream())
                            {
                                image.Encode(SKEncodedImageFormat.Png, 75).SaveTo(outputStream);
                                return outputStream.ToArray();
                            }
                        }
                    }
                }
            }
        }

    }
}
