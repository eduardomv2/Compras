using Compras.Models;
using Compras.Services;
using System.Windows.Input;

namespace Compras.ViewModels;

[QueryProperty(nameof(Producto), "Producto")]
public class ProductoDetalleViewModel : BaseViewModel
{
    private readonly CarritoService _carritoService;
    private readonly SesionService _sesionService;

    public ProductoDetalleViewModel(CarritoService carritoService, SesionService sesionService)
    {
        _carritoService = carritoService;
        _sesionService = sesionService;

        AgregarAlCarritoCommand = new Command(async () =>
        {
            if (!_sesionService.EstaAutenticado)
            {
                await Shell.Current.DisplayAlert(
                    "Inicia sesión",
                    "Necesitas iniciar sesión para agregar productos al carrito.",
                    "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            if (Producto is null) return;
            _carritoService.Agregar(Producto);
            await Shell.Current.DisplayAlert("Carrito",
                $"{Producto.Nombre} agregado al carrito.", "OK");
        });
    }

    private ProductoDto? _producto;
    public ProductoDto? Producto
    {
        get => _producto;
        set
        {
            _producto = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TieneDescuento));
            OnPropertyChanged(nameof(PrecioConDescuento));
            OnPropertyChanged(nameof(MontoDescuento));
            Titulo = value?.Nombre ?? string.Empty;
        }
    }

    public bool TieneDescuento => Producto is not null && Producto.PrecioVenta >= 10000;

    public decimal MontoDescuento => Producto is null ? 0m
        : Producto.PrecioVenta * (Producto.EsElectronica ? 0.05m : 0.10m);

    public decimal PrecioConDescuento => Producto is null ? 0m
        : Producto.PrecioVenta - MontoDescuento;

    public ICommand AgregarAlCarritoCommand { get; }
}