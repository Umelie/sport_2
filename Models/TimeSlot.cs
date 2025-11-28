using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace oculus_sport.Models;

// TimeSlot is used to display availability on the Schedule Page.
public partial class TimeSlot : ObservableObject
{
    // FIX 1 & 2: Changed TimeRange to SlotName and made it an ObservableProperty
    [ObservableProperty]
    private string _slotName = string.Empty; // e.g., "10:00 - 11:00"

    // Retained from your original structure (useful for internal calculations)
    public TimeSpan StartTime { get; set; }

    // Made IsAvailable an ObservableProperty for consistent MVVM binding
    [ObservableProperty]
    private bool _isAvailable = true;

    // Retained from your original structure
    [ObservableProperty]
    private bool _isSelected;
}