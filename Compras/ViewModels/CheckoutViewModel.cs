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

    private CreditoDto? _credito;

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

        SeleccionarDebitoCommand = new Command(() => EsDebito = true);

        SeleccionarCreditoCommand = new Command(async () =>
        {
            EsDebito = false;
            if (_credito is null)
            {
                _credito = await _clientesService.GetCreditoAsync(
                    _sesionService.UsuarioActual?.Id ?? 0);
                OnPropertyChanged(nameof(CreditoDisponibleTexto));
            }
        });

        SeleccionarMesesCommand = new Command<string>(meses =>
        {
            if (int.TryParse(meses, out var m))
                MesesSinIntereses = m;
        });
    }

    // ── Datos de tarjeta ──────────────────────────────────────────
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

    // ── Método de pago ────────────────────────────────────────────
    private bool _esDebito = true;
    public bool EsDebito
    {
        get => _esDebito;
        set
        {
            _esDebito = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EsCredito));
        }
    }
    public bool EsCredito => !EsDebito;

    private int _mesesSinIntereses = 3;
    public int MesesSinIntereses
    {
        get => _mesesSinIntereses;
        set
        {
            _mesesSinIntereses = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Meses3Seleccionado));
            OnPropertyChanged(nameof(Meses6Seleccionado));
            OnPropertyChanged(nameof(Meses12Seleccionado));
        }
    }

    public bool Meses3Seleccionado => MesesSinIntereses == 3;
    public bool Meses6Seleccionado => MesesSinIntereses == 6;
    public bool Meses12Seleccionado => MesesSinIntereses == 12;

    public string CreditoDisponibleTexto => _credito is null
        ? "Cargando..."
        : $"Disponible: ${_credito.CreditoDisponible:N2}";

    // ── Error ─────────────────────────────────────────────────────
    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    // ── Totales ───────────────────────────────────────────────────
    public decimal Total => _carritoService.Total;
    public decimal Descuento => _carritoService.DescuentoTotal;
    public decimal Subtotal => _carritoService.Subtotal;

    // ── Comandos ──────────────────────────────────────────────────
    public ICommand PagarCommand { get; }
    public ICommand SeleccionarDebitoCommand { get; }
    public ICommand SeleccionarCreditoCommand { get; }
    public ICommand SeleccionarMesesCommand { get; }

    private async Task PagarAsync()
    {
        if (EsDebito &&
            (string.IsNullOrWhiteSpace(NumeroTarjeta) ||
             string.IsNullOrWhiteSpace(NombreTarjeta) ||
             string.IsNullOrWhiteSpace(MesExpiracion) ||
             string.IsNullOrWhiteSpace(AnioExpiracion) ||
             string.IsNullOrWhiteSpace(Cvv)))
        {
            ErrorMensaje = "Todos los campos de la tarjeta son obligatorios.";
            return;
        }

        IsBusy = true;
        ErrorMensaje = string.Empty;

        var usuario = _sesionService.UsuarioActual!;

        // Verificar crédito disponible si aplica
        if (EsCredito)
        {
            _credito ??= await _clientesService.GetCreditoAsync(usuario.Id);
            if (_credito is null || _credito.CreditoDisponible < Total)
            {
                ErrorMensaje = $"Crédito insuficiente. Disponible: ${_credito?.CreditoDisponible:N2}";
                IsBusy = false;
                return;
            }
        }

        // Verificar stock
        foreach (var item in _carritoService.Items.ToList())
        {
            var producto = await _catalogoService.GetProductoAsync(item.Producto.Id);
            if (producto is not null && producto.Stock < item.Cantidad)
            {
                ErrorMensaje = $"Stock insuficiente para {item.Producto.Nombre}.";
                IsBusy = false;
                return;
            }
        }

        // 1. Crear la orden
        var (ordenExito, idOrden, ordenError) = await _ordenesService.CrearOrdenAsync(
            usuario.Id,
            idDireccionEnvio: 1,
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
        if (EsDebito)
        {
            // Pago con tarjeta via OpenPay
            var (pagoExito, pagoError) = await _pagosService.ProcesarPagoAsync(
                usuario.Id,
                NumeroTarjeta, NombreTarjeta,
                MesExpiracion, AnioExpiracion, Cvv,
                Total, 1, idOrden);

            if (!pagoExito)
            {
                ErrorMensaje = pagoError;
                IsBusy = false;
                return;
            }
        }
        else
        {
            // Pago con crédito de la tienda — no pasa por OpenPay
            var (creditoExito, mensajeCredito, errorCredito) =
                await _clientesService.RegistrarCompraCredito(
                    usuario.Id, Total,
                    $"Compra orden #{idOrden} a {MesesSinIntereses} MSI");

            if (!creditoExito)
            {
                ErrorMensaje = errorCredito;
                IsBusy = false;
                return;
            }

            if (!string.IsNullOrEmpty(mensajeCredito))
                await Shell.Current.DisplayAlert("Crédito", mensajeCredito, "OK");
        }

        // 4. Descontar stock
        foreach (var item in _carritoService.Items.ToList())
            await _catalogoService.DescontarStockAsync(item.Producto.Id, item.Cantidad);

        // 5. Obtener dirección y crear envío
        var direccion = await _clientesService.GetDireccionPrincipalAsync(usuario.Id);
        var direccionSnapshot = direccion?.DireccionCompleta ?? "Dirección no registrada";
        await _enviosService.CrearEnvioAsync(
            idOrden, direccionSnapshot, "Repartidor asignado", DateTime.UtcNow.AddDays(3));

        _carritoService.Limpiar();
        await Shell.Current.DisplayAlert(
            "¡Pago exitoso!",
            $"Tu pedido #{idOrden} ha sido procesado.", "OK");
        await Shell.Current.GoToAsync("//MisPedidosPage");

        IsBusy = false;
    }
}