using Compras.Models;
using Compras.Services;
using System.Windows.Input;


namespace Compras.ViewModels;

public class CheckoutViewModel : BaseViewModel
{
    private readonly CarritoService _carritoService;
    private readonly SesionService _sesionService;
    private readonly PagosService _pagosService;
    private readonly OrdenesService _ordenesService;
    private readonly EnviosService _enviosService;
    private readonly ClientesService _clientesService;
    private readonly CatalogoService _catalogoService;

    public CheckoutViewModel(
        CarritoService carritoService,
        SesionService sesionService,
        PagosService pagosService,
        OrdenesService ordenesService,
        EnviosService enviosService,
        ClientesService clientesService,
        CatalogoService catalogoService)
    {
        _carritoService = carritoService;
        _sesionService = sesionService;
        _pagosService = pagosService;
        _ordenesService = ordenesService;
        _enviosService = enviosService;
        _clientesService = clientesService;
        _catalogoService = catalogoService;
        Titulo = "Checkout";
        PagarCommand = new Command(async () => await PagarAsync());
    }

    private string _numeroTarjeta = string.Empty;
    public string NumeroTarjeta
    {
        get => _numeroTarjeta;
        set { _numeroTarjeta = value; OnPropertyChanged(); }
    }

    private string _nombreTarjeta = string.Empty;
    public string NombreTarjeta
    {
        get => _nombreTarjeta;
        set { _nombreTarjeta = value; OnPropertyChanged(); }
    }

    private string _mesExpiracion = string.Empty;
    public string MesExpiracion
    {
        get => _mesExpiracion;
        set { _mesExpiracion = value; OnPropertyChanged(); }
    }

    private string _anioExpiracion = string.Empty;
    public string AnioExpiracion
    {
        get => _anioExpiracion;
        set { _anioExpiracion = value; OnPropertyChanged(); }
    }

    private string _cvv = string.Empty;
    public string Cvv
    {
        get => _cvv;
        set { _cvv = value; OnPropertyChanged(); }
    }

    private int _mesesSinIntereses = 1;
    public int MesesSinIntereses
    {
        get => _mesesSinIntereses;
        set { _mesesSinIntereses = value; OnPropertyChanged(); }
    }

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    public decimal Total => _carritoService.Total;
    public decimal Descuento => _carritoService.DescuentoTotal;
    public decimal Subtotal => _carritoService.Subtotal;

    public ICommand PagarCommand { get; }

    private async Task PagarAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NumeroTarjeta) ||
            string.IsNullOrWhiteSpace(NombreTarjeta) ||
            string.IsNullOrWhiteSpace(MesExpiracion) ||
            string.IsNullOrWhiteSpace(AnioExpiracion) ||
            string.IsNullOrWhiteSpace(Cvv))
        {
            ErrorMensaje = "Todos los campos de la tarjeta son obligatorios.";
            return;
        }

        IsBusy = true;
        ErrorMensaje = string.Empty;

        // Verificar stock antes de pagar
        foreach (var item in _carritoService.Items.ToList())
        {
            var producto = await _catalogoService.GetProductoAsync(item.Producto.Id);
            if (producto is null || producto.Stock < item.Cantidad)
            {
                ErrorMensaje = $"Stock insuficiente para {item.Producto.Nombre}.";
                IsBusy = false;
                return;
            }
        }

        var usuario = _sesionService.UsuarioActual!;

        // 1. Crear la orden
        var (ordenExito, idOrden, ordenError) = await _ordenesService.CrearOrdenAsync(
            usuario.Id,
            idDireccionEnvio: 1, // temporal
            Subtotal,
            Descuento,
            Total,
            _carritoService.Items.ToList());

        if (!ordenExito)
        {
            ErrorMensaje = ordenError;
            IsBusy = false;
            return;
        }

        // 2. Procesar el pago
        var (pagoExito, pagoError) = await _pagosService.ProcesarPagoAsync(
            usuario.Id,
            NumeroTarjeta, NombreTarjeta,
            MesExpiracion, AnioExpiracion, Cvv,
            Total, MesesSinIntereses,
            idOrden);

        if (pagoExito)
        {
            // Descontar stock de cada producto
            foreach (var item in _carritoService.Items.ToList())
            {
                await _catalogoService.DescontarStockAsync(
                    item.Producto.Id, item.Cantidad);
            }

            // Obtener dirección del usuario
            var direccion = await _clientesService.GetDireccionPrincipalAsync(usuario.Id);
            var direccionSnapshot = direccion?.DireccionCompleta
                ?? "Dirección no registrada";

            var envioCreado = await _enviosService.CrearEnvioAsync(
                idOrden,
                direccionSnapshot,
                "Repartidor asignado",
                DateTime.UtcNow.AddDays(3));

            _carritoService.Limpiar();
            await Shell.Current.DisplayAlert(
                "¡Pago exitoso!",
                $"Tu pedido #{idOrden} ha sido procesado.", "OK");
            await Shell.Current.GoToAsync("//MisPedidosPage");
        }

        IsBusy = false;
    }
}