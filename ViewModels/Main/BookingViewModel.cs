using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Models;
using oculus_sport.ViewModels.Base;
using oculus_sport.Services;
using System.Diagnostics;
using System.Text.Json; // Required for serializing the Booking object
using oculus_sport.Services.Auth;

namespace oculus_sport.ViewModels.Main;

// This attribute allows us to receive the "Facility" object from the Home Page
[QueryProperty(nameof(Facility), "Facility")]
public partial class BookingViewModel : BaseViewModel
{
    private readonly IBookingService _bookingService;
    private readonly IAuthService _authService;

    // --- Observable Properties ---

    // NOTE: This property setter is the entry point for data passed from the previous page
    [ObservableProperty]
    private Facility _facility;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<TimeSlot> _timeSlots = new();

    // --- Constructor (Dependency Injection) ---

    public BookingViewModel(IBookingService bookingService, IAuthService authService)
    {
        _bookingService = bookingService;
        _authService = authService;
        Title = "Select Time";

        // Data will be loaded in the Facility setter after navigation completes
    }

    // --- Data Loading Logic ---

    // This partial method runs automatically when the Facility property is set by QueryProperty
    async partial void OnFacilityChanged(Facility value)
    {
        // Load the time slots as soon as we know which facility was selected
        await LoadTimeSlotsAsync();
    }

    // This partial method runs automatically when the SelectedDate property changes
    async partial void OnSelectedDateChanged(DateTime value)
    {
        // Only load if the date is in the future or today
        if (value.Date >= DateTime.Today.Date)
        {
            await LoadTimeSlotsAsync();
        }
        else
        {
            await Application.Current.MainPage!.DisplayAlert("Invalid Date", "Cannot select a past date.", "OK");
            SelectedDate = DateTime.Today;
        }
    }

    /// <summary>
    /// CRITICAL BACKEND INTEGRATION: Fetches real-time availability from the service layer.
    /// </summary>
    [RelayCommand]
    private async Task LoadTimeSlotsAsync()
    {
        if (IsBusy || Facility == null) return;
        IsBusy = true;
        TimeSlots.Clear();

        try
        {
            // 1. Check Auth State (Essential check before fetching personalized data)
            var user = _authService.GetCurrentUser();
            if (user == null)
            {
                // Optionally navigate to login or display a warning
            }

            // 2. Call IBookingService to fetch available slots
            Debug.WriteLine($"Fetching slots for {Facility.Name} on {SelectedDate.ToShortDateString()}");

            var availableSlots = await _bookingService.GetAvailableTimeSlotsAsync(Facility.Name, SelectedDate);

            // 3. Populate Observable Collection for the View
            foreach (var slot in availableSlots)
            {
                TimeSlots.Add(slot);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading time slots: {ex.Message}");
            await Application.Current.MainPage!.DisplayAlert("Error", "Failed to load court availability.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // --- User Interaction Commands ---

    [RelayCommand]
    void SelectSlot(TimeSlot slot)
    {
        if (slot == null || !slot.IsAvailable) return;

        // Unselect others (Single selection mode)
        foreach (var s in TimeSlots) s.IsSelected = false;

        slot.IsSelected = true;
    }

    [RelayCommand]
    async Task ConfirmBooking()
    {
        var selectedSlot = TimeSlots.FirstOrDefault(s => s.IsSelected);
        if (selectedSlot == null)
        {
            await Application.Current.MainPage!.DisplayAlert("Selection Required", "Please select an available time slot.", "OK");
            return;
        }

        // 1. Get current authenticated user ID
        var user = _authService.GetCurrentUser();
        if (user == null)
        {
            await Application.Current.MainPage!.DisplayAlert("Error", "Please sign in to make a booking.", "OK");
            return;
        }

        // 2. Create initial booking object
        var draftBooking = new Booking
        {
            UserId = user.Id, // CRITICAL FIX: Use authenticated ID
            FacilityName = Facility.Name,
            FacilityImage = Facility.ImageUrl,
            Location = Facility.Location,
            Date = SelectedDate.Date, // Use only the date part
            TimeSlot = selectedSlot.SlotName, // CRITICAL FIX: Use SlotName
            Status = "Draft"
        };

        // 3. Serialize object for navigation (safer than passing complex object directly)
        var bookingJson = JsonSerializer.Serialize(draftBooking);

        // 4. Navigate to Details Page (BookingDetailsViewModel)
        var navigationParameter = new Dictionary<string, object>
        {
            // Pass the serialized JSON string
            { "BookingData", bookingJson }
        };

        await Shell.Current.GoToAsync(nameof(Views.Main.BookingConfirmationPage), navigationParameter); // Navigate to confirmation page now
    }
}