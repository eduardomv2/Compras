using Compras.Models;
using Compras.Services;
using System.Windows.Input;

namespace Compras.ViewModels;

public class CheckoutViewModel : BaseViewModel
{
    private readonly CarritoService _carritoService;
    private readonly SesionService _sesionService;
    private readonly PagosService _pagosService;

    public CheckoutViewModel(
        CarritoService carritoService,
        SesionService sesionService,
        PagosService pagosService)
    {
        _carritoService = carritoService;
        _sesionService = sesionService;
        _pagosService = pagosService;
        Titulo = "Checkout";

        PagarCommand = new Command(async () => await PagarAsync());
    }

    // Datos de tarjeta
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

        var (exito, error) = await _pagosService.ProcesarPagoAsync(
            _sesionService.UsuarioActual!.Id,
            NumeroTarjeta, NombreTarjeta,
            MesExpiracion, AnioExpiracion, Cvv,
            Total, MesesSinIntereses);

        if (exito)
        {
            _carritoService.Limpiar();
            await Shell.Current.DisplayAlert(
                "¡Pago exitoso!", "Tu pedido ha sido procesado.", "OK");
            await Shell.Current.GoToAsync("//CatalogoPage");
        }
        else
        {
            ErrorMensaje = error;
        }

        IsBusy = false;
    }
}