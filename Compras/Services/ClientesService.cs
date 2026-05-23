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
        string email, string password, DateOnly fechaNacimiento)
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
                FechaNacimiento = fechaNacimiento
            };

            var response = await _http.PostAsJsonAsync(
                "/api/clientes/registro", dto);

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
}

public record ErrorDto(string? Error, string[]? Errores);