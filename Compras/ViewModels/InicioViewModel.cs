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
    private readonly SesionService _sesionService;

    private IDispatcherTimer? _timer;
    private int _promocionActual = 0;

    public ObservableCollection<PromocionDto> Promociones { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];
    public ObservableCollection<CategoriaDto> CategoriasDestacadas { get; } = [];
    public ObservableCollection<ProductoDto> Destacados { get; } = [];

    public int PromocionActual
    {
        get => _promocionActual;
        set { _promocionActual = value; OnPropertyChanged(); }
    }

    public InicioViewModel(
        PromocionesService promocionesService,
        CatalogoService catalogoService,
        OrdenesService ordenesService,
        SesionService sesionService)
    {
        _promocionesService = promocionesService;
        _catalogoService = catalogoService;
        _ordenesService = ordenesService;
        _sesionService = sesionService;
        Titulo = "Inicio";

        VerProductoCommand = new Command<ProductoDto>(async p =>
            await Shell.Current.GoToAsync(
                nameof(Views.ProductoDetallePage),
                new Dictionary<string, object> { ["Producto"] = p }));

        VerCatalogoCommand = new Command(async () =>
        {
            var ruta = _sesionService.EstaAutenticado
                ? "//CatalogoPage"
                : "//CatalogoPageInvitado";
            await Shell.Current.GoToAsync(ruta);
        });

        VerCategoriaCommand = new Command<CategoriaDto>(async categoria =>
        {
            var ruta = _sesionService.EstaAutenticado
                ? "//CatalogoPage"
                : "//CatalogoPageInvitado";
            await Shell.Current.GoToAsync(ruta,
                new Dictionary<string, object> { ["FiltroPromocion"] = categoria.Nombre });
        });

        VerPromocionCommand = new Command<PromocionDto>(async promo =>
        {
            var ruta = _sesionService.EstaAutenticado
                ? "//CatalogoPage"
                : "//CatalogoPageInvitado";
            await Shell.Current.GoToAsync(ruta,
                new Dictionary<string, object> { ["FiltroPromocion"] = promo.NombreCampana });
        });

        CargarCommand = new Command(async () => await CargarAsync());
        CargarCommand.Execute(null);
    }

    public ICommand VerProductoCommand { get; }
    public ICommand VerCatalogoCommand { get; }
    public ICommand VerCategoriaCommand { get; }
    public ICommand VerPromocionCommand { get; }
    public Command CargarCommand { get; }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var promociones = await _promocionesService.GetPromocionesActivasAsync();
        Promociones.Clear();
        foreach (var p in promociones.Take(5))
            Promociones.Add(p);

        // Timer carrusel
        _timer?.Stop();
        if (Promociones.Count > 1)
        {
            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += (s, e) =>
            {
                PromocionActual = (PromocionActual + 1) % Promociones.Count;
            };
            _timer.Start();
        }

        var categorias = await _catalogoService.GetCategoriasAsync();
        Categorias.Clear();
        CategoriasDestacadas.Clear();
        foreach (var c in categorias)
            Categorias.Add(c);
        foreach (var c in categorias.Take(4))
            CategoriasDestacadas.Add(c);

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

    public async Task RecargarAsync() => await CargarAsync();
}