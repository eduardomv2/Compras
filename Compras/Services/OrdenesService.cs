using Compras.Models;
using System.Net.Http.Json;

namespace Compras.Services;

public class OrdenesService
{
    private readonly HttpClient _http;

    public OrdenesService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<List<OrdenDto>> GetOrdenesPorUsuarioAsync(int idUsuario)
    {
        try
        {
            var ordenes = await _http.GetFromJsonAsync<List<OrdenDto>>(
                $"/api/ordenes/usuario/{idUsuario}");
            return ordenes ?? [];
        }
        catch
        {
            return [];
        }
    }
}