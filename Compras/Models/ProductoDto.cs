namespace Compras.Models;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public bool EsElectronica { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string? ImagenPortada { get; set; }
}