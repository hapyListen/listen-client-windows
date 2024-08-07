using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ListenTools.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return new SolidColorBrush(Color.Parse("#FFFFFF"));
        var paramColors = parameter.ToString().Split("|");
        
        return (bool)value ? new SolidColorBrush(Color.Parse(paramColors.FirstOrDefault() )) : 
                new SolidColorBrush(Color.Parse(paramColors.LastOrDefault()));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}