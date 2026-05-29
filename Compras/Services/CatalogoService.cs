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

    public async Task<PaginaProductosDto> GetProductosPaginadosAsync(int pagina = 1, int cantidad = 8)
    {
        try
        {
            var resultado = await _http.GetFromJsonAsync<PaginaProductosDto>(
                $"/api/catalogo/productos?pagina={pagina}&cantidad={cantidad}");
            return resultado ?? new PaginaProductosDto();
        }
        catch
        {
            return new PaginaProductosDto();
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

    public async Task<List<ProductoDto>> GetProductosAsync()
    {
        try
        {
            var resultado = await GetProductosPaginadosAsync(1, 100);
            return resultado.Productos;
        }
        catch
        {
            return [];
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

    public async Task<bool> DescontarStockAsync(int idProducto, int cantidad)
    {
        try
        {
            var dto = new { Cantidad = cantidad };
            var response = await _http.PatchAsJsonAsync(
                $"/api/catalogo/productos/{idProducto}/stock", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}