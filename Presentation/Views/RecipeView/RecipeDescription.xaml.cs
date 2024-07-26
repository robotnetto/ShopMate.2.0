using ShopMate._2._0.Presentation.ViewModels;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipeDescription : ContentPage
{
    private readonly RecipeViewModel recipeViewModel;

    public RecipeDescription(RecipeViewModel recipeViewModel)
	{
		InitializeComponent();
		BindingContext = recipeViewModel;
        this.recipeViewModel = recipeViewModel;
    }
    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (recipeViewModel != null && recipeViewModel.DebounceRecipeCommand.CanExecute(null))
        {
            recipeViewModel.DebounceRecipeCommand.Execute(null);
        }
    }
}