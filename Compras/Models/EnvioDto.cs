namespace Compras.Models;

public class EnvioDto
{
    public string Id { get; set; } = string.Empty;
    public int IdOrden { get; set; }
    public string DireccionSnapshot { get; set; } = string.Empty;
    public string GuiaPaqueteria { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;
    public string NombreRepartidor { get; set; } = string.Empty;
    public string? TelefonoRepartidor { get; set; }
    public DateTime FechaEstimada { get; set; }
    public DateTime? FechaEntregado { get; set; }
}

public class RastreoDto
{
    public string Estado { get; set; } = string.Empty;
    public string? Nota { get; set; }
    public DateTime FechaEvento { get; set; }
}