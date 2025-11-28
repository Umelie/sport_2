using System.Globalization;

namespace oculus_sport.Resources.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            // Return the color passed as a parameter (e.g., Primary Blue)
            return parameter as Color;
        }

        // Return default color when NOT selected (e.g., Secondary Light Blue or Transparent)
        // Using "Secondary" (Light Blue) makes unselected items look clickable but subtle.
        if (Application.Current.Resources.TryGetValue("Secondary", out var secondaryColor))
        {
            return (Color)secondaryColor;
        }

        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}