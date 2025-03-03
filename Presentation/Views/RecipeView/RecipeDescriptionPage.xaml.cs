using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShopMate._2._0.Presentation.ViewModels;
using ShopMate._2._0.Presentation.ViewModels.RecipeVm;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class RecipeDescriptionPage : ContentPage
{
    private readonly RecipeViewModel recipeViewModel;

    public RecipeDescriptionPage(RecipeViewModel recipeViewModel)
	{
        this.recipeViewModel = recipeViewModel;
		InitializeComponent();
        if (App.Current != null)
        {
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
        BindingContext = recipeViewModel;
        Shell.SetTabBarIsVisible(this, false);
        
        descriptionEditor.Unfocused += DescriptionEditor_Unfocused;
        
    }
    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (recipeViewModel != null && recipeViewModel.DebounceRecipeCommand.CanExecute(null))
        {
            recipeViewModel.DebounceRecipeCommand.Execute(null);
        }
    }
    //protected override  void OnDisappearing()
    //{
    //    base.OnDisappearing();
    //    if (descriptionEditor != null && descriptionEditor.Handler != null)
    //    {
    //        descriptionEditor.HideSoftInputAsync(default);
    //        descriptionEditor.Unfocus();
    //    }
    //}
    private void TapGestureRecognizer_Tapped(object sender, EventArgs a)
    {
       SafeUnfocusEditor();
    }
    private void SafeUnfocusEditor()
    {
        if (descriptionEditor != null && descriptionEditor.Handler != null)
        {
            descriptionEditor.Unfocus();
        }
    }
    //private void SetTabbarVisible(bool visible)
    //{
    //    //Shell.SetTabBarIsVisible(this, visible);
    //}
  
    //private void DescriptionEditor_Focused(object sender, FocusEventArgs e)
    //{
    //    SetTabbarVisible(false);
    //}

    private void DescriptionEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (descriptionEditor != null && descriptionEditor.Handler != null)
        {
            descriptionEditor.HideSoftInputAsync(default);
            descriptionEditor.Unfocus();
        }
    }
}