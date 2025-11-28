namespace oculus_sport.Models;

public class SportEvent
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DateDisplay { get; set; } = string.Empty; // e.g. "Nov 12" or "2 hours ago"
    public string ImageUrl { get; set; } = "dotnet_bot.png"; // Placeholder
    public bool IsNew { get; set; } // To show a "New" badge
}