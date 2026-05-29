using System.Globalization;

namespace Compras.Converters;

public class CategoriaDescripcionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Electrónica" => "Celulares, laptops y más",
            "Ropa Hombre" => "Estilo casual",
            "Ropa Mujer" => "Moda femenina",
            "Calzado" => "Para cada paso",
            "Hogar" => "Tu espacio ideal",
            "Juguetes" => "Diversión total",
            "Deportes" => "Actívate ya",
            "Belleza" => "Cuídate siempre",
            _ => "Descubre más"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}