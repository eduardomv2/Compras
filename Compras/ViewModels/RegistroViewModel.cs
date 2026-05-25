using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class RegistroViewModel : BaseViewModel
{
    private readonly ClientesService _clientesService;

    private CiudadOpcion? _ciudadSeleccionada;
    public CiudadOpcion? CiudadSeleccionada
    {
        get => _ciudadSeleccionada;
        set
        {
            _ciudadSeleccionada = value;
            OnPropertyChanged();
            if (value is not null)
                IdCiudadSeleccionada = value.Id;
        }
    }
    public RegistroViewModel(ClientesService clientesService)
    {
        _clientesService = clientesService;
        Titulo = "Crear cuenta";

        SiguienteCommand = new Command(async () => await SiguienteAsync());
        RegresarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        RegresarPaso1Command = new Command(() => EnPaso2 = false);
        RegistrarCommand = new Command(async () => await RegistrarAsync());

        CargarCiudades();
    }

    // ── Paso 1 ────
    private string _nombre = string.Empty;
    public string Nombre
    {
        get => _nombre;
        set { _nombre = value; OnPropertyChanged(); }
    }

    private string _apellidoPaterno = string.Empty;
    public string ApellidoPaterno
    {
        get => _apellidoPaterno;
        set { _apellidoPaterno = value; OnPropertyChanged(); }
    }

    private string _apellidoMaterno = string.Empty;
    public string ApellidoMaterno
    {
        get => _apellidoMaterno;
        set { _apellidoMaterno = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private string _confirmarPassword = string.Empty;
    public string ConfirmarPassword
    {
        get => _confirmarPassword;
        set { _confirmarPassword = value; OnPropertyChanged(); }
    }

    private DateTime _fechaNacimiento = DateTime.Today.AddYears(-18);
    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set { _fechaNacimiento = value; OnPropertyChanged(); }
    }

    // ── Paso 2 ─────
    private string _calleNumero = string.Empty;
    public string CalleNumero
    {
        get => _calleNumero;
        set { _calleNumero = value; OnPropertyChanged(); }
    }

    private string _colonia = string.Empty;
    public string Colonia
    {
        get => _colonia;
        set { _colonia = value; OnPropertyChanged(); }
    }

    private string _codigoPostal = string.Empty;
    public string CodigoPostal
    {
        get => _codigoPostal;
        set { _codigoPostal = value; OnPropertyChanged(); }
    }

    private int _idCiudadSeleccionada;
    public int IdCiudadSeleccionada
    {
        get => _idCiudadSeleccionada;
        set { _idCiudadSeleccionada = value; OnPropertyChanged(); }
    }

    public ObservableCollection<CiudadOpcion> Ciudades { get; } = [];

    // ── Control de pasos ────
    private bool _enPaso2;
    public bool EnPaso2
    {
        get => _enPaso2;
        set
        {
            _enPaso2 = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EnPaso1));
        }
    }
    public bool EnPaso1 => !EnPaso2;

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    public ICommand SiguienteCommand { get; }
    public ICommand RegresarCommand { get; }
    public ICommand RegresarPaso1Command { get; }
    public ICommand RegistrarCommand { get; }

    private void CargarCiudades()
    {
        Ciudades.Add(new CiudadOpcion(1, "Monclova"));
        Ciudades.Add(new CiudadOpcion(2, "Saltillo"));
        Ciudades.Add(new CiudadOpcion(3, "Torreón"));
        Ciudades.Add(new CiudadOpcion(4, "Piedras Negras"));
        Ciudades.Add(new CiudadOpcion(5, "Monterrey"));
        CiudadSeleccionada = Ciudades[4]; // Monterrey por defecto
    }

    private async Task SiguienteAsync()
    {
        ErrorMensaje = string.Empty;

        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(ApellidoPaterno) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmarPassword))
        {
            ErrorMensaje = "Todos los campos obligatorios deben llenarse.";
            return;
        }

        if (Password != ConfirmarPassword)
        {
            ErrorMensaje = "Las contraseñas no coinciden.";
            return;
        }

        EnPaso2 = true;
    }

    private async Task RegistrarAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(CalleNumero) ||
            string.IsNullOrWhiteSpace(Colonia) ||
            string.IsNullOrWhiteSpace(CodigoPostal))
        {
            ErrorMensaje = "Todos los campos de dirección son obligatorios.";
            return;
        }

        IsBusy = true;
        ErrorMensaje = string.Empty;

        var fechaSolo = DateOnly.FromDateTime(FechaNacimiento);

        var (exito, error) = await _clientesService.RegistrarAsync(
            Nombre, ApellidoPaterno,
            string.IsNullOrWhiteSpace(ApellidoMaterno) ? null : ApellidoMaterno,
            Email, Password, fechaSolo,
            IdCiudadSeleccionada, CalleNumero, Colonia, CodigoPostal);

        if (exito)
        {
            await Shell.Current.DisplayAlert(
                "¡Cuenta creada!", "Ya puedes iniciar sesión.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            ErrorMensaje = error;
            EnPaso2 = false;
        }

        IsBusy = false;
    }
}

public record CiudadOpcion(int Id, string Nombre);