//using CommunityToolkit.Maui.Core.Platform;
//using ShopMate._2._0.Presentation.ViewModels;
//using The49.Maui.BottomSheet;

//namespace ShopMate._2._0.Presentation.Views.RecipeView;

//public partial class EditBottomSheet : BottomSheet
//{
//	public EditBottomSheet(RecipeViewModel recipeViewModel)
//	{
//		InitializeComponent();
//		BindingContext = recipeViewModel;


//        editEntry.Focused += (s, e) => titleFrame.BorderColor = Color.FromArgb("#2e80ec");
//        Dismissed += CustomBottomSheet_Dismissed;

        
//    }

//    //private void CustomBottomSheet_Showing(object? sender, EventArgs e)
//    //{
//    //    if (titleEntry.Focus())
//    //    {
//    //        titleFrame.BorderColor = Color.FromArgb("#2e80ec");
//    //    }
//    //}

//    private void CustomBottomSheet_Dismissed(object? sender, DismissOrigin e)
//    {
//        HideKeyboard();
//    }

//    private void KeyboardLoaded()
//    {
//        editEntry.Focus();

//        editEntry.ShowSoftInputAsync(CancellationToken.None);

//    }
//    public async Task HideKeyboard()
//    {
//        await editEntry.HideKeyboardAsync(default);

//    }
//}