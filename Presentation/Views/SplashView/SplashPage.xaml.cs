using Microsoft.Extensions.Logging;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Repositories;

namespace ShopMate._2._0.Presentation.Views.SplashView;

public partial class SplashPage : ContentPage
{
    private readonly FoodDataService foodDataService;
   
    private SplashPage(FoodDataService foodDataService)
    {
        InitializeComponent();
        this.foodDataService = foodDataService;

        StartAnimate();
    }
    public SplashPage() : this(new FoodDataService(new FoodDataRepository(new Infrastructure.Data.LocalDbService())))
    {
        
    }

    private async void StartAnimate()
	{
            await Task.Run(async () => await foodDataService.PreloadDataAsync());
            await Task.Delay(3000);
            Application.Current!.MainPage = new AppShell();
        
       
   
    }
}