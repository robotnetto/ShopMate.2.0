using ShopMate._2._0.Presentation.ViewModels.CartVm;

namespace ShopMate._2._0.Presentation.Views.CartView;

public partial class ItemPage : ContentPage
{
    private readonly CartViewModel cartViewModel;

    public ItemPage(CartViewModel cartViewModel)
	{
		InitializeComponent();
        this.cartViewModel = cartViewModel;
        BindingContext = this.cartViewModel;
    }
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs  e )
    {
        OnPropertyChanged(nameof(cartViewModel));

    }
}