using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Models;
using oculus_sport.Services;
using oculus_sport.Services.Auth;
using oculus_sport.Services.Storage;
using oculus_sport.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace oculus_sport.ViewModels.Main;

public partial class HistoryPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IBookingService _bookingService;
    private readonly LocalDataService _localDataService;

    // Observable collection to display bookings in the View (HistoryPage.xaml)
    [ObservableProperty]
    private ObservableCollection<Booking> _bookings = new();

    public HistoryPageViewModel(
        IAuthService authService,
        IBookingService bookingService,
        LocalDataService localDataService)
    {
        Title = "Booking History";
        _authService = authService;
        _bookingService = bookingService;
        _localDataService = localDataService;
    }

    // --- Commands ---

    [RelayCommand]
    async Task LoadHistoryAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        
        try
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                Debug.WriteLine("User not logged in or ID missing.");
                // In a real app, this should navigate the user to the LoginPage.
                return;
            }

            // 1. OFFLINE FIRST: Load data immediately from SQLite
            var localBookings = await _localDataService.GetLocalBookingHistoryAsync(user.Id);
            Bookings.Clear();
            foreach (var booking in localBookings)
            {
                Bookings.Add(booking);
            }
            Debug.WriteLine($"Loaded {localBookings.Count} bookings from local SQLite cache.");

            // 2. ONLINE SYNC: If connected, fetch the latest data from Firebase
            if (Microsoft.Maui.Networking.Connectivity.Current.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
            {
                Debug.WriteLine("Online: Syncing with Firebase...");

                // This assumes GetUserBookingsAsync uses FirebaseDataService via the BookingService
                var onlineBookings = await _bookingService.GetUserBookingsAsync(user.Id);
                
                // Clear and refresh the display collection with the latest data
                Bookings.Clear();
                foreach (var booking in onlineBookings)
                {
                    Bookings.Add(booking);
                    
                    // 3. SYNCHRONIZATION: Update local SQLite cache
                    await _localDataService.SaveBookingToLocalCacheAsync(booking);
                }
                Debug.WriteLine($"Synced {onlineBookings.Count} bookings from Firebase.");
            }
            else
            {
                // If offline, the local data is already displayed.
                Debug.WriteLine("Offline mode. Displaying cached data.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load history: {ex.Message}");
            // Use CommunityToolkit.Maui.Alerts.Toast for user-friendly error messages
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // --- Lifecycle Integration ---

    // This method should be called from the View's OnAppearing to load data automatically.
    public void OnAppearing()
    {
        // Automatically fetch data when the user navigates to the History tab
        if (Bookings.Count == 0 || !IsBusy)
        {
            LoadHistoryCommand.Execute(null);
        }
    }
}