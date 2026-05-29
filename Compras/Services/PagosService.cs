using System.Net.Http.Json;

namespace Compras.Services;

public class PagosService
{
    private readonly HttpClient _http;

    public PagosService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<(bool Exito, string Error)> ProcesarPagoAsync(
        int idUsuario,
        string numeroTarjeta,
        string nombreTarjeta,
        string mes, string anio,
        string cvv,
        decimal monto,
        int mesesSinIntereses,
        int idOrden = 0)
    {
        try
        {

            // Registrar método de pago
            var metodoDto = new
            {
                IdUsuario = idUsuario,
                TokenIdOpenPay = $"tok_{numeroTarjeta[^4..]}",
                Ultimos4Digitos = numeroTarjeta[^4..],
                MarcaTarjeta = DetectarMarca(numeroTarjeta),
                EsCreditoTienda = false,
                EsPrincipal = true
            };

            var metodoResponse = await _http.PostAsJsonAsync(
                "/api/pagos/metodos", metodoDto);

            if (!metodoResponse.IsSuccessStatusCode)
                return (false, "Error al registrar método de pago.");

            var metodo = await metodoResponse.Content
                .ReadFromJsonAsync<MetodoCreado>();

            if (metodo is null)
                return (false, "Error al procesar el pago.");

            // Cobrar con datos completos de tarjeta
            var cobrarDto = new
            {
                IdOrden = idOrden,
                IdMetodoPago = metodo.Id,
                IdUsuario = idUsuario,
                Monto = monto,
                MesesSinIntereses = mesesSinIntereses,
                NumeroTarjeta = numeroTarjeta,
                NombreTarjeta = nombreTarjeta,
                MesExpiracion = mes,
                AnioExpiracion = anio,
                Cvv = cvv
            };

            var cobrarResponse = await _http.PostAsJsonAsync("/api/pagos/cobrar", cobrarDto);

            var contenidoLog = await cobrarResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"PagarCredito response: {cobrarResponse.StatusCode}, {contenidoLog}");

            if (cobrarResponse.IsSuccessStatusCode)
                return (true, string.Empty);

            var error = System.Text.Json.JsonSerializer.Deserialize<ErrorPago>(
                contenidoLog, new System.Text.Json.JsonSerializerOptions
                { PropertyNameCaseInsensitive = true });

            return (false, error?.Error ?? "Pago rechazado.");

        }
        catch
        {
            return (false, "No se pudo conectar al servidor de pagos.");
        }
    }

    private static string DetectarMarca(string numero)
    {
        if (numero.StartsWith("4")) return "Visa";
        if (numero.StartsWith("5")) return "Mastercard";
        if (numero.StartsWith("3")) return "Amex";
        return "Otra";
    }
}

record MetodoCreado(int Id, string MarcaTarjeta, string Ultimos4Digitos);
record ErrorPago(string? Error, string? Detalle);