using CommunityToolkit.Mvvm.ComponentModel;

namespace oculus_sport.Models;

public partial class SportCategory : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    // FIX: Use a private field. The Toolkit generates 'public bool IsSelected' for you.
    [ObservableProperty]
    private bool _isSelected;
}