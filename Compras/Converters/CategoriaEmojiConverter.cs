using System.Globalization;

namespace Compras.Converters;

public class CategoriaEmojiConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Electrónica" => "📱",
            "Ropa Hombre" => "👕",
            "Ropa Mujer" => "👗",
            "Calzado" => "👟",
            "Hogar" => "🏠",
            "Juguetes" => "🧸",
            "Deportes" => "⚽",
            "Belleza" => "💄",
            _ => "🛍️"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}