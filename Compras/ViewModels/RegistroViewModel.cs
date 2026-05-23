using Compras.Services;
using System.Windows.Input;

namespace Compras.ViewModels;

public class RegistroViewModel : BaseViewModel
{
    private readonly ClientesService _clientesService;

    public RegistroViewModel(ClientesService clientesService)
    {
        _clientesService = clientesService;
        Titulo = "Crear cuenta";

        RegistrarCommand = new Command(async () => await RegistrarAsync());
        RegresarCommand = new Command(async () =>
            await Shell.Current.GoToAsync(".."));
    }

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

    private DateTime _fechaNacimiento = DateTime.Today.AddYears(-18);
    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set { _fechaNacimiento = value; OnPropertyChanged(); }
    }

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    public ICommand RegistrarCommand { get; }
    public ICommand RegresarCommand { get; }

    private async Task RegistrarAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(ApellidoPaterno) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMensaje = "Todos los campos obligatorios deben llenarse.";
            return;
        }

        IsBusy = true;
        ErrorMensaje = string.Empty;

        var fechaSolo = DateOnly.FromDateTime(FechaNacimiento);

        var (exito, error) = await _clientesService.RegistrarAsync(
            Nombre, ApellidoPaterno,
            string.IsNullOrWhiteSpace(ApellidoMaterno) ? null : ApellidoMaterno,
            Email, Password, fechaSolo);

        if (exito)
        {
            await Shell.Current.DisplayAlert(
                "¡Cuenta creada!", "Ya puedes iniciar sesión.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            ErrorMensaje = error;
        }

        IsBusy = false;
    }
}