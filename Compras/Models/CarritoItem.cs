using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Compras.Models;

public class CarritoItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public ProductoDto Producto { get; set; } = null!;

    private int _cantidad = 1;
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            _cantidad = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Descuento));
            OnPropertyChanged(nameof(Total));
        }
    }

    public decimal Subtotal => Producto.PrecioVenta * Cantidad;

    public decimal Descuento => Subtotal >= 10000
        ? Subtotal * (Producto.EsElectronica ? 0.05m : 0.10m)
        : 0m;

    public decimal Total => Subtotal - Descuento;
}