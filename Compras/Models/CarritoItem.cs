namespace Compras.Models;

public class CarritoItem
{
    public ProductoDto Producto { get; set; } = null!;
    public int Cantidad { get; set; } = 1;

    public decimal Subtotal => Producto.PrecioVenta * Cantidad;

    public decimal Descuento => Subtotal >= 10000
        ? Subtotal * (Producto.EsElectronica ? 0.05m : 0.10m)
        : 0m;

    public decimal Total => Subtotal - Descuento;
}