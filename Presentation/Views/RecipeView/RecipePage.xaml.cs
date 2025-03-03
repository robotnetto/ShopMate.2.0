using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Services;
using ShopMate._2._0.Presentation.ViewModels.RecipeVm;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipePage : ContentPage
{
    private readonly RecipeViewModel recipeViewModel;
    public RecipePage(RecipeService recipeService, ImagePickerService imagePickerService )
	{
		InitializeComponent();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        this.recipeViewModel = new RecipeViewModel(recipeService, imagePickerService);
        recipeViewModel!.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
        BindingContext = recipeViewModel;
     

    }


}