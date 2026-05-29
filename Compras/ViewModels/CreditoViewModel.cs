using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class CreditoViewModel : BaseViewModel
{
    private readonly ClientesService _clientesService;
    private readonly SesionService _sesionService;

    public ObservableCollection<MovimientoDto> Movimientos { get; } = [];

    public CreditoViewModel(ClientesService clientesService, SesionService sesionService)
    {
        _clientesService = clientesService;
        _sesionService = sesionService;
        Titulo = "Mi Crédito";

        CargarCommand = new Command(async () => await CargarAsync());
        PagarCommand = new Command(async () => await PagarAsync());

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

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    public bool TieneSaldo => Credito is not null && Credito.SaldoUsado > 0;
    public double PorcentajeUso => Credito is null || Credito.LimiteCredito == 0
        ? 0 : (double)(Credito.SaldoUsado / Credito.LimiteCredito);

    public Command CargarCommand { get; }
    public ICommand PagarCommand { get; }
    public ICommand SolicitarCreditoCommand { get; }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var idUsuario = _sesionService.UsuarioActual?.Id ?? 0;
        Credito = await _clientesService.GetCreditoAsync(idUsuario);
        SinCredito = Credito is null;

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

        IsBusy = true;
        var idUsuario = _sesionService.UsuarioActual?.Id ?? 0;
        var response = await _clientesService.PagarCreditoAsync(idUsuario, monto);

        if (response.Exito)
        {
            MontoPago = string.Empty;
            await Shell.Current.DisplayAlert(
                "Pago exitoso", $"Pagaste ${monto:N2} a tu crédito.", "OK");
            await CargarAsync();
        }
        else
        {
            ErrorMensaje = response.Error;
        }

        IsBusy = false;
    }

    public async Task RecargarAsync() => await CargarAsync();
}