using Compras.Models;
using System.Net.Http.Json;

namespace Compras.Services;

public class CatalogoService
{
    private readonly HttpClient _http;

    public CatalogoService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<List<ProductoDto>> GetProductosAsync()
    {
        try
        {
            var productos = await _http.GetFromJsonAsync<List<ProductoDto>>(
                "/api/catalogo/productos");
            return productos ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<ProductoDto?> GetProductoAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProductoDto>(
                $"/api/catalogo/productos/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<CategoriaDto>> GetCategoriasAsync()
    {
        try
        {
            var categorias = await _http.GetFromJsonAsync<List<CategoriaDto>>(
                "/api/catalogo/categorias");
            return categorias ?? [];
        }
        catch
        {
            return [];
        }
    }
}