using Compras.Services;
using System.Windows.Input;

namespace Compras.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ClientesService _clientesService;
    private readonly SesionService _sesionService;

    public LoginViewModel(ClientesService clientesService, SesionService sesionService)
    {
        _clientesService = clientesService;
        _sesionService = sesionService;
        Titulo = "Iniciar Sesión";

        LoginCommand = new Command(async () => await LoginAsync());
        IrARegistroCommand = new Command(async () =>
            await Shell.Current.GoToAsync("RegistroPage"));
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

    private string _errorMensaje = string.Empty;
    public string ErrorMensaje
    {
        get => _errorMensaje;
        set { _errorMensaje = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }
    public ICommand IrARegistroCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMensaje = string.Empty;

        var (exito, usuario, error) = await _clientesService.LoginAsync(Email, Password);

        if (exito && usuario is not null)
        {
            _sesionService.IniciarSesion(usuario);
            await Shell.Current.GoToAsync("//CatalogoPage");
        }
        else
        {
            ErrorMensaje = error;
        }

        IsBusy = false;
    }
}