using CommunityToolkit.Maui.Core.Platform;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopMate._2._0.Presentation.ViewModels;
using The49.Maui.BottomSheet;



namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class AddEditBottomSheet : BottomSheet
{
    private readonly RecipeViewModel recipeView;

    public AddEditBottomSheet(RecipeViewModel recipeView)
    {
        InitializeComponent();
        BindingContext = recipeView;
       
         titleEntry.Focused += (s, e) => titleFrame.BorderColor = Color.FromArgb("#2e80ec");
        Dismissed += CustomBottomSheet_Dismissed;
        this.recipeView = recipeView;
    }

    //private void CustomBottomSheet_Showing(object? sender, EventArgs e)
    //{
    //    if (titleEntry.Focus())
    //    {
    //        titleFrame.BorderColor = Color.FromArgb("#2e80ec");
    //    }
    //}

    private void CustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
          HideKeyboard();
    }

    private void KeyboardLoaded()
    {
        titleEntry.Focus();
       
        titleEntry.ShowSoftInputAsync(CancellationToken.None);

    }
    public async void HideKeyboard()
    {
        await titleEntry.HideSoftInputAsync(default);
        
    }

}