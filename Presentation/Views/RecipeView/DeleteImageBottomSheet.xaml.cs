using ShopMate._2._0.Presentation.ViewModels.RecipeVm;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class DeleteImageBottomSheet : BottomSheet
{
	public DeleteImageBottomSheet(RecipeViewModel recipeViewModel)
	{
		InitializeComponent();
        BindingContext = recipeViewModel;
    }
}