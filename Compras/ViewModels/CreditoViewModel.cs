using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class CreditoViewModel : BaseViewModel
{
    private readonly ClientesService _clientesService;
    private readonly SesionService _sesionService;
    private readonly PagosService _pagosService;

    public ObservableCollection<MovimientoDto> Movimientos { get; } = [];

    public CreditoViewModel(
        ClientesService clientesService,
        SesionService sesionService,
        PagosService pagosService)
    {
        _clientesService = clientesService;
        _sesionService = sesionService;
        _pagosService = pagosService;
        Titulo = "Mi Crédito";

        CargarCommand = new Command(async () => await CargarAsync());
        PagarCommand = new Command(async () => await PagarAsync());
        MostrarPagoCommand = new Command(() => MostrarFormularioPago = true);
        UsarPagoMinimoCommand = new Command(() =>
        {
            if (Credito is not null)
                MontoPago = Credito.PagoMinimo.ToString("F2");
        });

        SolicitarCreditoCommand = new Command(async () =>
        {
            IsBusy = true;
            var idUsuario = _sesionService.UsuarioActual?.Id ?? 0;
            var (exito, mensaje, error) = await _clientesService.SolicitarCreditoAsync(idUsuario);

            if (exito)
            {
                await Shell.Current.DisplayAlert("¡Crédito aprobado!", mensaje, "OK");
                IsBusy = false;
                await CargarAsync();
            }
            else
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("No disponible", error, "OK");
            }
        });

        CargarCommand.Execute(null);
    }

    private CreditoDto? _credito;
    public CreditoDto? Credito
    {
        get => _credito;
        set
        {
            _credito = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TieneSaldo));
            OnPropertyChanged(nameof(PorcentajeUso));
        }
    }

    private bool _sinCredito;
    public bool SinCredito
    {
        get => _sinCredito;
        set { _sinCredito = value; OnPropertyChanged(); }
    }

    private string _montoPago = string.Empty;
    public string MontoPago
    {
        get => _montoPago;
        set { _montoPago = value; OnPropertyChanged(); }
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

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    private bool _mostrarFormularioPago;
    public bool MostrarFormularioPago
    {
        get => _mostrarFormularioPago;
        set { _mostrarFormularioPago = value; OnPropertyChanged(); }
    }

    public ICommand MostrarPagoCommand { get; }

    

    public bool TieneSaldo => Credito is not null && Credito.SaldoUsado > 0;
    public double PorcentajeUso => Credito is null || Credito.LimiteCredito == 0
        ? 0 : (double)(Credito.SaldoUsado / Credito.LimiteCredito);

    public Command CargarCommand { get; }
    public ICommand PagarCommand { get; }
    public ICommand SolicitarCreditoCommand { get; }
    public ICommand UsarPagoMinimoCommand { get; }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var idUsuario = _sesionService.UsuarioActual?.Id ?? 0;
        Credito = await _clientesService.GetCreditoAsync(idUsuario);
        SinCredito = Credito is null;

        // Verificar si aplica reducción de tasa automáticamente
        if (Credito is not null && Credito.TasaInteresAnual > 0.10m)
        {
            var (exito, mensaje, _) = await _clientesService.RevisarTasaCreditoAsync(idUsuario);
            if (exito)
            {
                Credito = await _clientesService.GetCreditoAsync(idUsuario);
                await Shell.Current.DisplayAlert("¡Buenas noticias!", mensaje, "OK");
            }
        }

        var movimientos = await _clientesService.GetMovimientosAsync(idUsuario);
        Movimientos.Clear();
        foreach (var m in movimientos)
            Movimientos.Add(m);

        IsBusy = false;
    }

    private async Task PagarAsync()
    {
        if (IsBusy) return;
        ErrorMensaje = string.Empty;

        if (!decimal.TryParse(MontoPago, out var monto) || monto <= 0)
        {
            ErrorMensaje = "Ingresa un monto válido.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NumeroTarjeta) ||
            string.IsNullOrWhiteSpace(NombreTarjeta) ||
            string.IsNullOrWhiteSpace(MesExpiracion) ||
            string.IsNullOrWhiteSpace(AnioExpiracion) ||
            string.IsNullOrWhiteSpace(Cvv))
        {
            ErrorMensaje = "Ingresa los datos de tu tarjeta.";
            return;
        }

        IsBusy = true;
        var idUsuario = _sesionService.UsuarioActual?.Id ?? 0;

        // Procesar pago con OpenPay
        var (pagoExito, pagoError) = await _pagosService.ProcesarPagoAsync(
            idUsuario,
            NumeroTarjeta, NombreTarjeta,
            MesExpiracion, AnioExpiracion, Cvv,
            monto, 1, 0);

        if (!pagoExito)
        {
            ErrorMensaje = pagoError;
            IsBusy = false;
            return;
        }

        // Registrar pago en crédito
        var (creditoExito, creditoError) = await _clientesService.PagarCreditoAsync(
            idUsuario, monto);

        if (creditoExito)
        {
            MostrarFormularioPago = false;
            MontoPago = string.Empty;
            NumeroTarjeta = string.Empty;
            NombreTarjeta = string.Empty;
            MesExpiracion = string.Empty;
            AnioExpiracion = string.Empty;
            Cvv = string.Empty;
            await Shell.Current.DisplayAlert(
                "Pago exitoso", $"Pagaste ${monto:N2} a tu crédito.", "OK");
            IsBusy = false;
            await CargarAsync();
        }
        else
        {
            ErrorMensaje = creditoError;
            IsBusy = false;
        }
    }

    public async Task RecargarAsync() => await CargarAsync();
}