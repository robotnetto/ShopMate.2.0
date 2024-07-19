using ShopMate._2._0.Presentation.ViewModels;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.RecipeView;

public partial class OptionsBottomSheet : BottomSheet
{
	public OptionsBottomSheet(RecipeViewModel recipeViewModel)
	{
		InitializeComponent();
		BindingContext = recipeViewModel;
	}
}