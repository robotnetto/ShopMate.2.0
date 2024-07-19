using CommunityToolkit.Maui.Core.Platform;
using ShopMate._2._0.Presentation.ViewModels;
using The49.Maui.BottomSheet;



namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class AddBottomSheet : BottomSheet
{
    public AddBottomSheet(RecipeViewModel recipeView)
    {
        InitializeComponent();
        BindingContext = recipeView;

        Dismissed += CustomBottomSheet_Dismissed;
    }

    private void CustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
    {
        HideKeyboard();
    }

    private void KeyboardLoaded()
    {
        titleEntry.Focus();
        titleEntry.ShowSoftInputAsync(CancellationToken.None);

    }
    public async Task HideKeyboard()
    {
        await titleEntry.HideKeyboardAsync(default);
    }

}