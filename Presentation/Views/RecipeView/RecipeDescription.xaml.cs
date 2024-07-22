using ShopMate._2._0.Presentation.ViewModels;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipeDescription : ContentPage
{
	public RecipeDescription(RecipeViewModel recipeViewModel)
	{
		InitializeComponent();
		BindingContext = recipeViewModel;

	}
}