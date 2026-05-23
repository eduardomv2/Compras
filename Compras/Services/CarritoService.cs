using Compras.Models;

namespace Compras.Services;

public class CarritoService
{
    private readonly List<CarritoItem> _items = [];

    public IReadOnlyList<CarritoItem> Items => _items;
    public int TotalItems => _items.Sum(i => i.Cantidad);

    public decimal Subtotal => _items.Sum(i => i.Subtotal);
    public decimal DescuentoTotal => _items.Sum(i => i.Descuento);
    public decimal Total => _items.Sum(i => i.Total);

    public event Action? CarritoCambio;

    public void Agregar(ProductoDto producto)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item is not null)
            item.Cantidad++;
        else
            _items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });

        CarritoCambio?.Invoke();
    }

    public void Quitar(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item is null) return;

        if (item.Cantidad > 1)
            item.Cantidad--;
        else
            _items.Remove(item);

        CarritoCambio?.Invoke();
    }

    public void Eliminar(int productoId)
    {
        _items.RemoveAll(i => i.Producto.Id == productoId);
        CarritoCambio?.Invoke();
    }

    public void Limpiar()
    {
        _items.Clear();
        CarritoCambio?.Invoke();
    }
}