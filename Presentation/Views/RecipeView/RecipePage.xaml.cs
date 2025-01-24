using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Presentation.ViewModels.RecipeVm;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipePage : ContentPage
{

    public RecipePage( )
	{
		InitializeComponent();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        var recipeViewModel = new RecipeViewModel();
        recipeViewModel.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
		BindingContext = recipeViewModel;
    }

}