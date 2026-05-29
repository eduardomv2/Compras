namespace Compras.Models;

public class PromocionDto
{
    public string Id { get; set; } = string.Empty;
    public string NombreCampana { get; set; } = string.Empty;
    public decimal PorcentajeDescuento { get; set; }
    public decimal MontoMinimoCompra { get; set; }
    public bool EstaVigente { get; set; }
    public string TextoDescuento => $"{PorcentajeDescuento:0}% OFF";
    public string TextoMinimo => MontoMinimoCompra > 0
        ? $"En compras mayores a ${MontoMinimoCompra:N0}"
        : "En todos los productos";
}