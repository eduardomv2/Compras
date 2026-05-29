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

    public async Task<List<int>> GetProductosMasVendidosAsync()
    {
        try
        {
            var ids = await _http.GetFromJsonAsync<List<int>>(
                "/api/ordenes/productos-mas-vendidos");
            return ids ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool Exito, int IdOrden, string Error)> CrearOrdenAsync(
    int idUsuario,
    int idDireccionEnvio,
    decimal subtotal,
    decimal descuento,
    decimal total,
    List<CarritoItem> items)
    {
        try
        {
            var claveIdempotencia = Guid.NewGuid().ToString();

            var dto = new
            {
                IdUsuario = idUsuario,
                IdDireccionEnvio = idDireccionEnvio,
                Subtotal = subtotal,
                DescuentoAplicado = descuento,
                TotalFinal = total,
                ClaveIdempotencia = claveIdempotencia,
                Detalles = items.Select(i => new
                {
                    IdProducto = i.Producto.Id,
                    NombreProducto = i.Producto.Nombre,
                    EsElectronica = i.Producto.EsElectronica,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.Producto.PrecioVenta,
                    DescuentoLinea = i.Descuento
                }).ToList()
            };

            var response = await _http.PostAsJsonAsync("/api/ordenes", dto);

            if (response.IsSuccessStatusCode)
            {
                var orden = await response.Content
                    .ReadFromJsonAsync<OrdenCreadaDto>();
                return (true, orden?.Id ?? 0, string.Empty);
            }

            return (false, 0, "Error al crear la orden.");
        }
        catch
        {
            return (false, 0, "No se pudo conectar al servidor de órdenes.");
        }
    }

    record OrdenCreadaDto(int Id, decimal TotalFinal, string ClaveIdempotencia);
}