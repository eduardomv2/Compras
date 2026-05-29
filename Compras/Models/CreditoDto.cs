namespace Compras.Models;

public class CreditoDto
{
    public int Id { get; set; }
    public decimal LimiteCredito { get; set; }
    public decimal SaldoUsado { get; set; }
    public decimal CreditoDisponible { get; set; }
    public decimal TasaInteresAnual { get; set; }
    public decimal InteresesAcumulados { get; set; }
    public decimal InteresesMensuales { get; set; }
    public int TotalCompras { get; set; }
    public string FechaApertura { get; set; } = string.Empty;
    public decimal PagoMinimo => Math.Round(SaldoUsado / 12, 2);
    public decimal TotalMensual => PagoMinimo + InteresesMensuales;
}

public class ResultadoCreditoDto
{
    public decimal SaldoUsado { get; set; }
    public decimal CreditoDisponible { get; set; }
    public decimal LimiteCredito { get; set; }
    public int TotalCompras { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

public class MovimientoDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}