using Compras.Models;
using System.Net.Http.Json;

namespace Compras.Services;

public class PromocionesService
{
    private readonly HttpClient _http;

    public PromocionesService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<List<PromocionDto>> GetPromocionesActivasAsync()
    {
        try
        {
            var promociones = await _http.GetFromJsonAsync<List<PromocionDto>>(
                "/api/promociones/activas");
            return promociones ?? [];
        }
        catch
        {
            return [];
        }
    }
}