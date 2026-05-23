using Compras.Models;

namespace Compras.Services;

public class SesionService
{
    public UsuarioDto? UsuarioActual { get; private set; }
    public bool EstaAutenticado => UsuarioActual is not null;

    public void IniciarSesion(UsuarioDto usuario)
    {
        UsuarioActual = usuario;
    }

    public void CerrarSesion()
    {
        UsuarioActual = null;
    }
}