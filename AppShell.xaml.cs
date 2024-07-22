using ShopMate._2._0.Presentation.Views;
using ShopMate._2._0.Presentation.Views.RecipeView;

namespace ShopMate._2._0
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RecipeDescription), typeof(RecipeDescription));

        }

    }
}
