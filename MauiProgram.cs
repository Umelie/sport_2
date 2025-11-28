using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using oculus_sport.Services;
using oculus_sport.Services.Auth;
using oculus_sport.Services.Storage;
using oculus_sport.ViewModels.Auth;
using oculus_sport.ViewModels.Main;
using oculus_sport.Views.Auth;
using oculus_sport.Views.Main;

// These using statements are for Firebase initialization and service
using Plugin.Firebase;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Storage;

namespace oculus_sport;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ---------------------------------------------------------
        // ❗ NO Firebase initialization yet
        //    (avoids errors since platform setup is not done)
        // ---------------------------------------------------------
        // CrossFirebase.Initialize();  ← remove for now
        // ---------------------------------------------------------

        // 1. Services
        builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
        builder.Services.AddSingleton<IDatabaseService, FirebaseDataService>();
        builder.Services.AddSingleton<Services.Other.ConnectivityService>();
        builder.Services.AddSingleton<IBookingService, BookingService>();
        builder.Services.AddSingleton<LocalDataService>();

        // 2. ViewModels
        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<SignUpPageViewModel>();
        builder.Services.AddTransient<HomePageViewModel>();
        builder.Services.AddTransient<SchedulePageViewModel>();
        builder.Services.AddTransient<EventPageViewModel>();
        builder.Services.AddTransient<ProfilePageViewModel>();
        builder.Services.AddTransient<HistoryPageViewModel>();
        builder.Services.AddTransient<BookingViewModel>();
        builder.Services.AddTransient<BookingDetailsViewModel>();
        builder.Services.AddTransient<BookingConfirmationViewModel>();
        builder.Services.AddTransient<BookingSuccessViewModel>();

        // 3. Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SignUpPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SchedulePage>();
        builder.Services.AddTransient<EventPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<BookingPage>();
        builder.Services.AddTransient<Views.Main.BookingDetailsPage>();
        builder.Services.AddTransient<Views.Main.BookingConfirmationPage>();
        builder.Services.AddTransient<Views.Main.BookingSuccessPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
