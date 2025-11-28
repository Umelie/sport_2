using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Models;
using oculus_sport.ViewModels.Base;

namespace oculus_sport.ViewModels.Main;

public partial class SchedulePageViewModel : BaseViewModel
{
    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Now;

    [ObservableProperty]
    private ObservableCollection<SportCategory> _categories = new();

    // We will reuse the Facility model but we might dynamically populate "Price" or a new field with availability info
    [ObservableProperty]
    private ObservableCollection<Facility> _availableFacilities = new();

    public SchedulePageViewModel()
    {
        Title = "Check Availability";
        LoadCategories();
        LoadAvailability(); // Initial load
    }

    private void LoadCategories()
    {
        Categories = new ObservableCollection<SportCategory>
        {
            new SportCategory { Name = "Pickleball", IsSelected = true },
            new SportCategory { Name = "Badminton" },
            new SportCategory { Name = "Football" },
            new SportCategory { Name = "Tennis" }
        };
    }

    // When Date changes, reload availability
    async partial void OnSelectedDateChanged(DateTime value)
    {
        await LoadAvailability();
    }

    [RelayCommand]
    async Task SelectCategory(SportCategory category)
    {
        if (Categories == null) return;
        foreach (var c in Categories) c.IsSelected = false;
        category.IsSelected = true;

        // Refresh UI hack
        var temp = Categories; Categories = null; Categories = temp;

        await LoadAvailability();
    }

    private async Task LoadAvailability()
    {
        IsBusy = true;
        await Task.Delay(500); // Simulate API call // remove this after integrating real API

        AvailableFacilities.Clear();

        // MOCK DATA: In a real app, you'd query the DB for this Sport + Date

        // We would want to remove this after firebase database has been created

        // We will show that some courts have specific slots open

        AvailableFacilities.Add(new Facility
        {
            Name = "Court Interlock 1",
            Location = "9:00, 10:00, 14:00", // Reusing 'Location' to show slots for now
            ImageUrl = "badminton_court.png",
            Rating = 4.5
        });

        AvailableFacilities.Add(new Facility
        {
            Name = "Court Vinyl 2",
            Location = "11:00, 12:00, 16:00",
            ImageUrl = "badminton_court.png",
            Rating = 4.8
        });

        IsBusy = false;
    }
}