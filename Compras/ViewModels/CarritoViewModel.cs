using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class CarritoViewModel : BaseViewModel
{
    private readonly CarritoService _carritoService;

    public ObservableCollection<CarritoItem> Items { get; } = [];

    public CarritoViewModel(CarritoService carritoService)
    {
        _carritoService = carritoService;
        Titulo = "Mi Carrito";

        AumentarCommand = new Command<CarritoItem>(item =>
        {
            _carritoService.Agregar(item.Producto);
            Actualizar();
        });

        DisminuirCommand = new Command<CarritoItem>(item =>
        {
            _carritoService.Quitar(item.Producto.Id);
            Actualizar();
        });

        EliminarCommand = new Command<CarritoItem>(item =>
        {
            _carritoService.Eliminar(item.Producto.Id);
            Actualizar();
        });

        _carritoService.CarritoCambio += Actualizar;
        Actualizar();
    }

    public ICommand AumentarCommand { get; }
    public ICommand DisminuirCommand { get; }
    public ICommand EliminarCommand { get; }

    public decimal Subtotal => _carritoService.Subtotal;
    public decimal DescuentoTotal => _carritoService.DescuentoTotal;
    public decimal Total => _carritoService.Total;
    public bool TieneDescuento => DescuentoTotal > 0;
    public bool CarritoVacio => !Items.Any();
    public int TotalItems => _carritoService.TotalItems; 

    private void Actualizar()
    {
        Items.Clear();
        foreach (var item in _carritoService.Items)
            Items.Add(item);

        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(DescuentoTotal));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TieneDescuento));
        OnPropertyChanged(nameof(CarritoVacio));
        OnPropertyChanged(nameof(TotalItems));
    }

    
}