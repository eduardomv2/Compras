namespace Compras.Models;

public class DireccionDto
{
    public int Id { get; set; }
    public string CalleNumero { get; set; } = string.Empty;
    public string Colonia { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;

    public string DireccionCompleta =>
        $"{CalleNumero}, {Colonia}, CP {CodigoPostal}, {Ciudad}";
}