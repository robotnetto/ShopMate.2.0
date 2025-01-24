using ShopMate._2._0.Presentation.ViewModels.RecipeVm;
using ShopMate._2._0.Presentation.Views.RecipeView;
using ShopMate._2._0.Presentation.Views.SplashView;

namespace ShopMate._2._0
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new SplashPage();
            //var services = new ServiceCollection();
            //services.AddSingleton<RecipeViewModel>();
            //var serviceProvider = services.BuildServiceProvider();

            //MainPage = new NavigationPage(new RecipePage(serviceProvider.GetService<RecipeViewModel>()));
        }
    }
}
