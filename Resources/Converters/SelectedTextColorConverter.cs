using System.Globalization;

namespace oculus_sport.Resources.Converters;

public class SelectedTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return Colors.White; // Selected Text
        }

        return Colors.Black; // Unselected Text
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}