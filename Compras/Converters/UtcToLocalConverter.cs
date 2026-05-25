using System.Globalization;

namespace Compras.Converters;

public class UtcToLocalConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime fecha)
        {
            var fechaUtc = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);

            // Usar zona horaria de México Centro
            var zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
            var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, zonaHoraria);

            return fechaLocal.ToString("dd/MM/yyyy HH:mm");
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}