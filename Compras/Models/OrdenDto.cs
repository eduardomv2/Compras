namespace Compras.Models;

public class OrdenDto
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public int IdEstadoOrden { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DescuentoAplicado { get; set; }
    public decimal TotalFinal { get; set; }
    public string ClaveIdempotencia { get; set; } = string.Empty;
    public EstadoOrdenDto? EstadoOrden { get; set; }
    public List<DetalleOrdenDto> Detalles { get; set; } = [];
}

public class EstadoOrdenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class DetalleOrdenDto
{
    public int Id { get; set; }
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal DescuentoLinea { get; set; }
}