using Microsoft.Extensions.Logging;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Data;
using ShopMate._2._0.Infrastructure.Repositories;

namespace ShopMate._2._0
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddDbContext<LocalDbService>();
            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddScoped<IShopCartRepository, ShopCartRepository>();
            builder.Services.AddScoped<ShopCartService>();

            var dbContext = new LocalDbService();
            //dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
