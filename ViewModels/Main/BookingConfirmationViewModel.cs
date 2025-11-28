using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Models;
using oculus_sport.Services;
using oculus_sport.Services.Auth;
using oculus_sport.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json; // Used for serializing/deserializing complex objects

namespace oculus_sport.ViewModels.Main;

// IMPORTANT: [QueryProperty] is how MAUI Shell passes data between ViewModels during navigation.
// The key "BookingData" must match the parameter name used in Shell.Current.GoToAsync(...)
[QueryProperty(nameof(BookingData), "BookingData")]
public partial class BookingConfirmationViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IBookingService _bookingService;

    // --- Observable Properties ---

    // Property to receive and hold the Booking object passed from the previous page
    [ObservableProperty]
    private Booking _currentBooking = new();

    // Property to display the final calculated cost on the confirmation screen
    [ObservableProperty]
    private string _finalCostDisplay = "Calculating...";

    // --- Query Property Setter ---

    // This setter receives the raw data (usually a JSON string) from the previous VM 
    // and deserializes it into the CurrentBooking object.
    public string BookingData
    {
        set
        {
            try
            {
                // Deserialize the passed JSON string back into a Booking object
                // Uri.UnescapeDataString handles characters that might be URL encoded
                var booking = JsonSerializer.Deserialize<Booking>(Uri.UnescapeDataString(value));
                if (booking != null)
                {
                    CurrentBooking = booking;
                    Title = $"Confirm: {booking.FacilityName}";
                    // Trigger the cost calculation immediately after receiving the data
                    // We use GetAwaiter().GetResult() here because property setters cannot be async.
                    CalculateCostAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to deserialize booking data: {ex.Message}");
            }
        }
    }

    // --- Constructor ---

    public BookingConfirmationViewModel(IAuthService authService, IBookingService bookingService)
    {
        _authService = authService;
        _bookingService = bookingService;
        Title = "Confirm Booking";
    }

    // --- Logic Commands ---

    [RelayCommand]
    async Task CalculateCostAsync()
    {
        // Prevent recalculating if we already have the cost, unless explicitly triggered.
        if (CurrentBooking.TotalCost != "Calculating..." && CurrentBooking.TotalCost != "Rp 50.000") return;

        try
        {
            // Call the IBookingService method to apply discounts and rules dynamically
            FinalCostDisplay = await _bookingService.CalculateFinalCostAsync(
                CurrentBooking.FacilityName,
                CurrentBooking.TimeSlot,
                CurrentBooking.ContactStudentId ?? string.Empty);

            // Update the internal model property for persistence
            CurrentBooking.TotalCost = FinalCostDisplay;
        }
        catch (Exception ex)
        {
            FinalCostDisplay = "Rp 0.00 (Error)";
            Debug.WriteLine($"Failed to calculate cost: {ex.Message}");
        }
    }

    [RelayCommand]
    async Task ConfirmBookingAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                // Safety net: User must be authenticated to book.
                await Application.Current.MainPage!.DisplayAlert("Error", "You must be logged in to book.", "OK");
                return;
            }

            // 1. Finalize Booking Object
            CurrentBooking.UserId = user.Id;
            CurrentBooking.TotalCost = FinalCostDisplay; // Use the calculated cost

            // 2. CRITICAL TRANSACTIONAL CALL: Calls the service that does the secure check, payment simulation, and write to Firebase.
            // This is the core logic that prevents double booking.
            var confirmedBooking = await _bookingService.ProcessAndConfirmBookingAsync(CurrentBooking);

            if (confirmedBooking != null)
            {
                // 3. Success! Navigate to the confirmation screen, passing the confirmed details.
                // NOTE: We pass the whole object back, which will include the confirmed status and final cost.
                var navigationParameter = new Dictionary<string, object>
                {
                    { "BookingData", JsonSerializer.Serialize(confirmedBooking) }
                };

                await Shell.Current.GoToAsync($"//{nameof(Views.Main.BookingSuccessPage)}", navigationParameter);
            }
            else
            {
                // Transaction failed (e.g., slot taken by another user in the interim)
                await Application.Current.MainPage!.DisplayAlert("Booking Failed",
                    "The selected time slot is no longer available, or the transaction failed.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage!.DisplayAlert("Error", $"Booking failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task CancelBooking()
    {
        // Go back one page in the navigation stack
        await Shell.Current.GoToAsync("..");
    }
}