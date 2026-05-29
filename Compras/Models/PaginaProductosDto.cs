namespace Compras.Models;

public class PaginaProductosDto
{
    public List<ProductoDto> Productos { get; set; } = [];
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int Cantidad { get; set; }
    public bool HayMas { get; set; }
}