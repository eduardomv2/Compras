using System.Globalization;

namespace Compras.Converters;

public class ElectronicaDescuentoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool esElectronica && esElectronica ? "5% OFF" : "10% OFF";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}