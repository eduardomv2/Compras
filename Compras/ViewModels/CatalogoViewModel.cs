using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Compras.Views;

namespace Compras.ViewModels;

[QueryProperty(nameof(FiltroPromocion), "FiltroPromocion")]
public class CatalogoViewModel : BaseViewModel
{
    private string _filtroPromocion = string.Empty;
    public string FiltroPromocion
    {
        get => _filtroPromocion;
        set
        {
            _filtroPromocion = value;
            OnPropertyChanged();
            if (!string.IsNullOrEmpty(value))
            {
                Busqueda = value;
                FiltrarProductos();
            }
        }
    }
    private readonly CatalogoService _catalogoService;
    private readonly CarritoService _carritoService;

    public ObservableCollection<ProductoDto> Productos { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    public CatalogoViewModel(CatalogoService catalogoService, CarritoService carritoService)
    {
        _catalogoService = catalogoService;
        _carritoService = carritoService;
        Titulo = "Catálogo";

        CargarCommand = new Command(async () => await CargarAsync());
        VerProductoCommand = new Command<ProductoDto>(async p =>
            await Shell.Current.GoToAsync(
                nameof(ProductoDetallePage),
                new Dictionary<string, object> { ["Producto"] = p }));
        FiltrarPorCategoriaCommand = new Command<CategoriaDto>(categoria =>
        {
            Busqueda = categoria.Nombre;
        });

        CargarCommand.Execute(null);
    }

    public Command CargarCommand { get; }
    public ICommand VerProductoCommand { get; }
    public ICommand FiltrarPorCategoriaCommand { get; }

    private string _busqueda = string.Empty;
    public string Busqueda
    {
        get => _busqueda;
        set
        {
            _busqueda = value;
            OnPropertyChanged();
            FiltrarProductos();
        }
    }

    private List<ProductoDto> _todosLosProductos = [];

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var productos = await _catalogoService.GetProductosAsync();
        var categorias = await _catalogoService.GetCategoriasAsync();

        _todosLosProductos = productos;
        Productos.Clear();
        foreach (var p in productos)
            Productos.Add(p);

        Categorias.Clear();
        foreach (var c in categorias.DistinctBy(c => c.Nombre))
            Categorias.Add(c);

        IsBusy = false;
    }

    private void FiltrarProductos()
    {
        var filtrados = string.IsNullOrWhiteSpace(Busqueda)
            ? _todosLosProductos
            : _todosLosProductos.Where(p =>
                p.Nombre.Contains(Busqueda, StringComparison.OrdinalIgnoreCase) ||
                p.Categoria.Contains(Busqueda, StringComparison.OrdinalIgnoreCase) == true);

        Productos.Clear();
        foreach (var p in filtrados)
            Productos.Add(p);
    }

    public async Task RecargarAsync()
    {
        await CargarAsync();
    }
}