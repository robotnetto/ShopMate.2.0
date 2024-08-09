using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Presentation.ViewModels.CartVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class CartPage : ContentPage
{
    public CartPage( )
    {
        InitializeComponent();
        BindingContext = new CartViewModel();
    }
}
