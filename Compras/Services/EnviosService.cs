using Compras.Models;
using System.Net.Http.Json;

namespace Compras.Services;

public class EnviosService
{
    private readonly HttpClient _http;

    public EnviosService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<EnvioDto?> GetEnvioPorOrdenAsync(int idOrden)
    {
        try
        {
            return await _http.GetFromJsonAsync<EnvioDto>(
                $"/api/envios/orden/{idOrden}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CrearEnvioAsync(
    int idOrden,
    string direccionSnapshot,
    string nombreRepartidor,
    DateTime fechaEstimada)
    {
        try
        {
            var dto = new
            {
                IdOrden = idOrden,
                DireccionSnapshot = direccionSnapshot,
                NombreRepartidor = nombreRepartidor,
                FechaEstimada = fechaEstimada
            };
            var response = await _http.PostAsJsonAsync("/api/envios", dto);
            var contenido = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CrearEnvio status: {response.StatusCode}, body: {contenido}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CrearEnvio error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<RastreoDto>> GetHistorialRastreoAsync(string idEnvio)
    {
        try
        {
            var historial = await _http.GetFromJsonAsync<List<RastreoDto>>(
                $"/api/envios/{idEnvio}/rastreo");
            return historial ?? [];
        }
        catch
        {
            return [];
        }
    }
}