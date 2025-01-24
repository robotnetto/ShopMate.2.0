using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace ShopMate._2._0
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Change the status bar color
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FFFFFF")); // Replace with your desired color

            // Optional: Set the status bar text/icons to light or dark
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar; // Use LightStatusBar for dark text/icons
        }
    }
   

}
