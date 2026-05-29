using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class InicioViewModel : BaseViewModel
{
    private readonly PromocionesService _promocionesService;
    private readonly CatalogoService _catalogoService;
    private readonly OrdenesService _ordenesService;

    public ObservableCollection<PromocionDto> Promociones { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];
    public ObservableCollection<ProductoDto> Destacados { get; } = [];

    public ObservableCollection<CategoriaDto> CategoriasDestacadas { get; } = [];

    public InicioViewModel(
        PromocionesService promocionesService,
        CatalogoService catalogoService,
        OrdenesService ordenesService)
    {
        _promocionesService = promocionesService;
        _catalogoService = catalogoService;
        _ordenesService = ordenesService;
        Titulo = "Inicio";

        VerProductoCommand = new Command<ProductoDto>(async p =>
            await Shell.Current.GoToAsync(
                nameof(Views.ProductoDetallePage),
                new Dictionary<string, object> { ["Producto"] = p }));


        VerCatalogoCommand = new Command(async () =>
        {
            // Navegar al tab de Catálogo sin salir del TabBar
            var shell = Shell.Current;
            await shell.GoToAsync("//CatalogoPage");
        });

        CargarCommand = new Command(async () => await CargarAsync());
        CargarCommand.Execute(null);
    }

    public ICommand VerProductoCommand { get; }
    public ICommand VerCatalogoCommand { get; }
    public Command CargarCommand { get; }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        // Cargar promociones
        var promociones = await _promocionesService.GetPromocionesActivasAsync();
        Promociones.Clear();
        foreach (var p in promociones.Take(5))
            Promociones.Add(p);

        // Cargar categorías
        var categorias = await _catalogoService.GetCategoriasAsync();
        Categorias.Clear();
        CategoriasDestacadas.Clear();
        foreach (var c in categorias)
            Categorias.Add(c);
        foreach (var c in categorias.Take(4))
            CategoriasDestacadas.Add(c);

        // Cargar destacados en paralelo
        var tareaDestacados = _ordenesService.GetProductosMasVendidosAsync();
        var tareaProductos = _catalogoService.GetProductosAsync();
        await Task.WhenAll(tareaDestacados, tareaProductos);

        var idsDestacados = tareaDestacados.Result;
        var todosProductos = tareaProductos.Result;

        var destacados = new List<ProductoDto>();
        foreach (var id in idsDestacados)
        {
            var producto = todosProductos.FirstOrDefault(p => p.Id == id);
            if (producto is not null)
                destacados.Add(producto);
        }
        foreach (var producto in todosProductos)
        {
            if (destacados.Count >= 6) break;
            if (!destacados.Any(d => d.Id == producto.Id))
                destacados.Add(producto);
        }

        Destacados.Clear();
        foreach (var d in destacados)
            Destacados.Add(d);

        IsBusy = false;
    }

    public async Task RecargarAsync()
    {
        await CargarAsync();
    }
}