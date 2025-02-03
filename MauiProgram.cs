using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;
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
            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddScoped<IShopCartRepository, CartRepository>();
            builder.Services.AddScoped<IFoodDataRepository, FoodDataRepository>();
            builder.Services.AddSingleton<FoodDataService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<RecipeService>();
           



            //var dbContext = new LocalDbService();
            ////dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //dbContext.Dispose();
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbService>();
                //dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

            //PreloadFoodData(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        //private static void PreloadFoodData(IServiceCollection services)
        //{
        //    using (var scope = services.BuildServiceProvider().CreateScope())
        //    {
        //        var foodDataService = scope.ServiceProvider.GetRequiredService<FoodDataService>();
        //        Task.Run(async () =>
        //        {
        //            // Preload food data asynchronously
        //            await foodDataService.PreloadDataAsync();
        //        }).ConfigureAwait(false);
        //    }
        //}
    }
}
