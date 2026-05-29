using Compras.Models;
using System.Net.Http.Json;

namespace Compras.Services;

public class ClientesService
{
    private readonly HttpClient _http;

    public ClientesService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<(bool Exito, UsuarioDto? Usuario, string Error)> LoginAsync(
        string email, string password)
    {
        try
        {
            var dto = new LoginDto { Email = email, Password = password };
            var response = await _http.PostAsJsonAsync("/api/clientes/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content
                    .ReadFromJsonAsync<UsuarioDto>();
                return (true, usuario, string.Empty);
            }

            return (false, null, "Correo o contraseña incorrectos.");
        }
        catch
        {
            return (false, null, "No se pudo conectar al servidor.");
        }
    }

    public async Task<(bool Exito, string Error)> RegistrarAsync(
        string nombre, string apellidoPaterno, string? apellidoMaterno,
        string email, string password, DateOnly fechaNacimiento,
        int idCiudad, string calleNumero, string colonia, string codigoPostal)
    {
        try
        {
            var dto = new
            {
                Nombre = nombre,
                ApellidoPaterno = apellidoPaterno,
                ApellidoMaterno = apellidoMaterno,
                Email = email,
                Password = password,
                FechaNacimiento = fechaNacimiento,
                Direccion = new
                {
                    IdCiudad = idCiudad,
                    CalleNumero = calleNumero,
                    Colonia = colonia,
                    CodigoPostal = codigoPostal
                }
            };

            var response = await _http.PostAsJsonAsync("/api/clientes/registro", dto);

            if (response.IsSuccessStatusCode)
                return (true, string.Empty);

            var error = await response.Content.ReadFromJsonAsync<ErrorDto>();
            var mensaje = error?.Error
                ?? (error?.Errores != null ? string.Join("\n", error.Errores) : "Error al registrar.");
            return (false, mensaje);
        }
        catch
        {
            return (false, "No se pudo conectar al servidor.");
        }
    }

    public async Task<DireccionDto?> GetDireccionPrincipalAsync(int idUsuario)
    {
        try
        {
            return await _http.GetFromJsonAsync<DireccionDto>(
                $"/api/clientes/{idUsuario}/direccion-principal");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Exito, string Mensaje, string Error)> RegistrarCompraCredito(
        int idUsuario, decimal monto, string descripcion)
    {
        try
        {
            var dto = new { Monto = monto, Descripcion = descripcion };
            var response = await _http.PostAsJsonAsync(
                $"/api/clientes/{idUsuario}/credito/compra", dto);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content
                    .ReadFromJsonAsync<ResultadoCreditoDto>();
                return (true, resultado?.Mensaje ?? string.Empty, string.Empty);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorDto>();
            return (false, string.Empty, error?.Error ?? "Error al registrar compra.");
        }
        catch
        {
            return (false, string.Empty, "No se pudo conectar al servidor.");
        }
    }

    public async Task<CreditoDto?> GetCreditoAsync(int idUsuario)
    {
        try
        {
            return await _http.GetFromJsonAsync<CreditoDto>(
                $"/api/clientes/{idUsuario}/credito");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<MovimientoDto>> GetMovimientosAsync(int idUsuario)
    {
        try
        {
            var movimientos = await _http.GetFromJsonAsync<List<MovimientoDto>>(
                $"/api/clientes/{idUsuario}/credito/movimientos");
            return movimientos ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool Exito, string Error)> PagarCreditoAsync(
        int idUsuario, decimal monto)
    {
        try
        {
            var dto = new { Monto = monto };
            var response = await _http.PostAsJsonAsync(
                $"/api/clientes/{idUsuario}/credito/pago", dto);

            if (response.IsSuccessStatusCode)
                return (true, string.Empty);

            var error = await response.Content.ReadFromJsonAsync<ErrorDto>();
            return (false, error?.Error ?? "Error al realizar el pago.");
        }
        catch
        {
            return (false, "No se pudo conectar al servidor.");
        }
    }

    public async Task<(bool Exito, string Mensaje, string Error)> SolicitarCreditoAsync(
        int idUsuario)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(
                $"/api/clientes/{idUsuario}/credito/solicitar", new { });

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content
                    .ReadFromJsonAsync<ResultadoCreditoDto>();
                return (true, resultado?.Mensaje ?? "Crédito aprobado.", string.Empty);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorDto>();
            return (false, string.Empty, error?.Error ?? "Error al solicitar crédito.");
        }
        catch
        {
            return (false, string.Empty, "No se pudo conectar al servidor.");
        }
    }
}

public record ErrorDto(string? Error, string[]? Errores);