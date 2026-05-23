namespace Compras.Models;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool TieneCredito { get; set; }
}