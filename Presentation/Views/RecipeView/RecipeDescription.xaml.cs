using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Presentation.ViewModels;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipeDescription : ContentPage
{
    private readonly RecipeViewModel recipeViewModel;

    public RecipeDescription(RecipeViewModel recipeViewModel)
	{
		InitializeComponent();
        if (App.Current != null)
        {
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
        BindingContext = recipeViewModel;
        this.recipeViewModel = recipeViewModel;
        descriptionEditor.Unfocused +=  (x, y) => OnDisappearing();
    }
    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (recipeViewModel != null && recipeViewModel.DebounceRecipeCommand.CanExecute(null))
        {
            recipeViewModel.DebounceRecipeCommand.Execute(null);
        }
    }
    protected override  void OnDisappearing()
    {
        base.OnDisappearing();
        descriptionEditor.HideSoftInputAsync(default);
    }

}