using Foundation;
using Plugin.Firebase.Core.Platforms.iOS;
using UIKit;

namespace oculus_sport
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize Firebase iOS SDK (fully qualified to avoid collision with MAUI App class)
            Firebase.Core.App.Configure();
            CrossFirebase.Initialize();
            return base.FinishedLaunching(app, options);
        }
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}