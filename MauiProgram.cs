using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ItemVm;
using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;
using ShopMate._2._0.Presentation.Views.CartView;
using ShopMate._2._0.Presentation.Views.ProfileView;
using ShopMate._2._0.Presentation.Views.SplashView;
using SkiaSharp.Views.Maui.Controls.Hosting;
using The49.Maui.BottomSheet;

namespace ShopMate._2._0
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseBottomSheet()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Logging.AddDebug();
            builder.Services.AddDbContext<LocalDbService>();
            builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
            builder.Services.AddSingleton<ICartRepository, CartRepository>();
            builder.Services.AddSingleton<IProfileRepository, ProfileRepository>();
            builder.Services.AddSingleton<IFoodDataRepository, FoodDataRepository>();

            // Register services
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<ProfileService>();
            builder.Services.AddSingleton<FoodDataService>();

            // Register ViewModels
            builder.Services.AddSingleton<CartViewModel>();
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ItemViewModel>();


            // Register Pages
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<ItemPage>();

            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbService>();
                //dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

          

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

     
    }
}
