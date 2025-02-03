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
using System.Text;
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
        FileResult? _imageStream;

        private BottomSheetMode CurrentBottomSheetMode { get; set; }
        private BottomSheet CurrentBottomSheet { get; set; }
        public ICommand AddRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UpdateRecipeCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CardSelectCommand { get; }
        public ICommand DebounceRecipeCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand DeleteImageOptionCommand { get; }
        public ICommand DeleteImageCommand { get; }
        public ICommand ShareRecipeCommand { get; }
        private readonly RecipeService _recipeService;
        private RecipeViewModel(RecipeService recipeService)
        {


            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            AddRecipeCommand = new AsyncRelayCommand(OnAddCommandAsync);
            RemoveRecipeCommand = new AsyncRelayCommand(OnRemoveRecipeAsync);
            UpdateRecipeCommand = new AsyncRelayCommand(OnUpdateCommandAsync);
            OptionsCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnOptionsBottomSheetAsync!);
            CloseCommand = new AsyncRelayCommand(OnCloseBottomSheetAsync);
            SaveRecipeCommand = new AsyncRelayCommand(OnSaveAndEditRecipeAsync);
            CardSelectCommand = new AsyncRelayCommand<RecipeDetailsViewModel>(OnNavigateAsync!);
            DebounceRecipeCommand = new DebounceCommand(SaveRecipeCommand, TimeSpan.FromSeconds(1));
            LoadImageCommand = new AsyncRelayCommand(ImageUploadAsync);
            DeleteImageOptionCommand = new AsyncRelayCommand(OnDeleteImageCommandAsync);
            DeleteImageCommand = new AsyncRelayCommand(OnDeleteImageAsync);
            ShareRecipeCommand = new AsyncRelayCommand(OnShareRecipeAsync);

            _ = InitializeDataAsync();

        }
        public RecipeViewModel() : this(new RecipeService(new RecipeRepository(new LocalDbService())))
        {
        }


        private async Task InitializeDataAsync()
        {
            var allRecipes = await _recipeService.GetAllRecipesAsync();
            foreach (var recipe in allRecipes)
            {
                var recipeVm = new RecipeDetailsViewModel(recipe);
                Recipes.Add(recipeVm);
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
            Console.WriteLine("Navigating to RecipeDescription");

            SelectedRecipe = recipeDetailsViewModel;
            CurrentBottomSheetMode = BottomSheetMode.EditDescription;
            await Shell.Current.Navigation.PushAsync(new RecipeDescriptionPage(this));
       
        }

        private async Task OnOptionsBottomSheetAsync(RecipeDetailsViewModel recipeDetailsViewModel)
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
                //BottomSheetTitle = nameof(BottomSheetMode.Edit);
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
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
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
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
            finally { semaphoreSlim?.Release(); }

        }

        private async Task OnSaveNewRecipeAsync()
        {

            Recipe newRecipe = new() { Title = NewRecipeTitle, Favorite = false };
            await _recipeService.AddNewRecipeAsync(newRecipe);
            var recipeVm = new RecipeDetailsViewModel(newRecipe);
            Recipes.Add(recipeVm);
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
                selectedRecipe.Favorite = SelectedRecipe.Favorite;
                selectedRecipe.ImageStream = SelectedRecipe.ImageStream;

                await _recipeService.UpdateExistingRecipeAsync(selectedRecipe);

                var updatedRecipe = await _recipeService.GetRecipeByIdAsync(selectedRecipe.Id);

                SelectedRecipe.Title = updatedRecipe.Title;
                SelectedRecipe.Favorite = updatedRecipe.Favorite;
                SelectedRecipe.Description = updatedRecipe.Description;
                SelectedRecipe.ImageStream = updatedRecipe.ImageStream;
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
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }
            try
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
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
            }
            finally
            {
                semaphoreSlim.Release();
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
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return;
            }

            try
            {
                Console.WriteLine("Starting ImageUpload");

                _imageStream = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select a photo" });

                if (_imageStream == null)
                {
                    Console.WriteLine("No image selected");
                    return;
                }

                Console.WriteLine("Image selected");

                using (var stream = await _imageStream.OpenReadAsync())
                {
                    byte[] result;
                    using (var streamReader = new MemoryStream())
                    {
                        await stream.CopyToAsync(streamReader);
                        result = streamReader.ToArray();
                    }

                    // Resize the image using SkiaSharp
                    byte[] resizedImage = ResizeImage(result, 530, 310); // Resize to 530x310

                    var imagePath = Convert.ToBase64String(resizedImage);
                    imagePath = string.Format("data:image/png;base64,{0}", imagePath);

                    SelectedRecipe.ImageStream = imagePath;
                }

                Console.WriteLine("Image processed and assigned");

                await OnUpdateRecipeAsync();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Image upload failed: {ex.Message}");
            }
            finally
            {
                semaphoreSlim.Release();
                Console.WriteLine("ImageUpload completed");
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
