using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.Views;
using ShopMate._2._0.Presentation.Views.CartView;
using ShopMate._2._0.Presentation.Views.ProfileView;
using ShopMate._2._0.Presentation.Views.RecipeView;

namespace ShopMate._2._0
{
    public partial class AppShell : Shell
    {
        //public static CartViewModel? GlobalCartViewModel { get; set; }

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemPage), typeof(ItemPage));
            Routing.RegisterRoute(nameof(FoodDataPage), typeof(FoodDataPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

        }

    }


}
