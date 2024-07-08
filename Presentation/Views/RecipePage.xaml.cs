using ShopMate._2._0.Presentation.ViewModels;

namespace ShopMate._2._0.Presentation.Views;

public partial class RecipePage : ContentPage
{
	public RecipePage()
	{
		InitializeComponent();

        var recipeView = new RecipeViewModel();
		recipeView.ErrorOccurred += async (message) => await DisplayAlert("Error", message, "OK");
		BindingContext = recipeView;
	}

   
}