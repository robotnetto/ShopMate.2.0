namespace ShopMate._2._0.Presentation.Views.SplashView;

public partial class SplashPage : ContentPage
{
	public SplashPage()
	{
		InitializeComponent();
		StartAnimate();
    }

	private async void StartAnimate()
	{
        await Task.Delay(3000);
		Application.Current!.MainPage = new AppShell();
    }
}