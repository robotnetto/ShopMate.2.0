using ShopMate._2._0.Presentation.ViewModels.CartVm;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartOptionsBottomSheet : BottomSheet
{
	public CartOptionsBottomSheet(CartViewModel cartViewModel)
	{
		InitializeComponent();
		BindingContext = cartViewModel;
	}
}