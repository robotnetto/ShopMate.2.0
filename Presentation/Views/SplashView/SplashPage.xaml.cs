using Microsoft.Extensions.Logging;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Repositories;
using System.ComponentModel;

namespace ShopMate._2._0.Presentation.Views.SplashView;

public partial class SplashPage : ContentPage, INotifyPropertyChanged
{
    private readonly FoodDataService foodDataService;
    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set
        {
            if(isLoading != value)
            isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }
    private SplashPage(FoodDataService foodDataService)
    {
        InitializeComponent();
        this.foodDataService = foodDataService;
        BindingContext = this;
        StartAnimate();
    }
    public SplashPage() : this(new FoodDataService(new FoodDataRepository(new Infrastructure.Data.LocalDbService())))
    {
        
    }

    private async void StartAnimate()
	{
        var result = await foodDataService.GetAllAsync();

        if (!result.Any())
        {
            IsLoading = true;
            await Task.Run(async () => await foodDataService.PreloadDataAsync());
           
        }
        await Task.Delay(1000);
        IsLoading = false;
        Application.Current!.MainPage = new AppShell();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}