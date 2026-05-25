using Compras.Models;
using Compras.Services;
using System.Windows.Input;

namespace Compras.ViewModels;

[QueryProperty(nameof(Producto), "Producto")]
public class ProductoDetalleViewModel : BaseViewModel
{
    private readonly CarritoService _carritoService;
    private readonly SesionService _sesionService;
    private readonly CatalogoService _catalogoService;

    public ProductoDetalleViewModel(CarritoService carritoService, SesionService sesionService, CatalogoService catalogoService)
    {
        _carritoService = carritoService;
        _sesionService = sesionService;
        _catalogoService = catalogoService;

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

            if (Producto.Stock <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Sin stock",
                    "Este producto no está disponible por el momento.",
                    "OK");
                return;
            }

            _carritoService.Agregar(Producto);
            await Shell.Current.DisplayAlert("Carrito",
                $"{Producto.Nombre} agregado al carrito.", "OK");
        });
        _catalogoService = catalogoService;

        ComprarAhoraCommand = new Command(async () =>
        {
            if (!_sesionService.EstaAutenticado)
            {
                await Shell.Current.DisplayAlert(
                    "Inicia sesión",
                    "Necesitas iniciar sesión para comprar.",
                    "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            if (Producto is null) return;

            if (Producto.Stock <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Sin stock",
                    "Este producto no está disponible por el momento.",
                    "OK");
                return;
            }

            _carritoService.Agregar(Producto);
            await Shell.Current.GoToAsync("CheckoutPage");
        });
    }
    public async Task RecargarProductoAsync()
    {
        if (Producto is null) return;
        var actualizado = await _catalogoService.GetProductoAsync(Producto.Id);
        if (actualizado is not null)
            Producto = actualizado;
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
            OnPropertyChanged(nameof(HayStock));
            OnPropertyChanged(nameof(TextoStock));
            OnPropertyChanged(nameof(ColorStock));
            Titulo = value?.Nombre ?? string.Empty;
        }
    }


    public bool TieneDescuento => Producto is not null && Producto.PrecioVenta >= 10000;

    public decimal MontoDescuento => Producto is null ? 0m
        : Producto.PrecioVenta * (Producto.EsElectronica ? 0.05m : 0.10m);

    public decimal PrecioConDescuento => Producto is null ? 0m
        : Producto.PrecioVenta - MontoDescuento;

    public bool HayStock => Producto is not null && Producto.Stock > 0;
    public string TextoStock => Producto is null ? string.Empty
        : Producto.Stock == 0 ? "Sin stock" : $"{Producto.Stock} disponibles";
    public Color ColorStock => Producto?.Stock == 0 ? Colors.Red : Color.FromArgb("#2ECC71");

    public ICommand AgregarAlCarritoCommand { get; }

    public ICommand ComprarAhoraCommand { get; }
}