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
                Busqueda = value;
        }
    }

    private readonly CatalogoService _catalogoService;
    private readonly CarritoService _carritoService;

    public ObservableCollection<ProductoDto> Productos { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    private int _paginaActual = 1;
    private bool _hayMas = true;

    private bool _cargandoMas;
    public bool CargandoMas
    {
        get => _cargandoMas;
        set { _cargandoMas = value; OnPropertyChanged(); }
    }

    public bool HayMas => _hayMas && string.IsNullOrWhiteSpace(Busqueda);

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
        CargarMasCommand = new Command(async () => await CargarMasAsync(),
            () => _hayMas && !CargandoMas);

        CargarCommand.Execute(null);
    }

    public Command CargarCommand { get; }
    public ICommand VerProductoCommand { get; }
    public ICommand FiltrarPorCategoriaCommand { get; }
    public ICommand CargarMasCommand { get; }

    private async Task FiltrarConBusquedaAsync()
    {
        if (string.IsNullOrWhiteSpace(Busqueda))
        {
            // Sin búsqueda: mostrar solo la página actual
            FiltrarProductos();
            return;
        }

        // Con búsqueda: traer todos y filtrar
        IsBusy = true;
        var todos = await _catalogoService.GetProductosAsync();
        _todosLosProductos = todos;
        FiltrarProductos();
        IsBusy = false;
    }

    private string _busqueda = string.Empty;
    public string Busqueda
    {
        get => _busqueda;
        set
        {
            _busqueda = value;
            OnPropertyChanged();
            _ = FiltrarConBusquedaAsync();
        }
    }

    private List<ProductoDto> _todosLosProductos = [];

    

    private async Task CargarMasAsync()
    {
        if (CargandoMas || !_hayMas) return;
        CargandoMas = true;

        _paginaActual++;
        var resultado = await _catalogoService.GetProductosPaginadosAsync(_paginaActual);
        _hayMas = resultado.HayMas;

        foreach (var p in resultado.Productos)
        {
            if (!_todosLosProductos.Any(x => x.Id == p.Id))
            {
                _todosLosProductos.Add(p);
            }
        }

        FiltrarProductos();
        ((Command)CargarMasCommand).ChangeCanExecute();
        CargandoMas = false;
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

        OnPropertyChanged(nameof(HayMas));
    }

    public async Task RecargarAsync()
    {
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        _paginaActual = 1;
        _hayMas = true;

        var resultado = await _catalogoService.GetProductosPaginadosAsync(_paginaActual);
        _todosLosProductos = resultado.Productos;
        _hayMas = resultado.HayMas;

        var categorias = await _catalogoService.GetCategoriasAsync();
        Categorias.Clear();
        foreach (var c in categorias.DistinctBy(c => c.Nombre))
            Categorias.Add(c);

        // Aplicar filtro pendiente si existe
        if (!string.IsNullOrEmpty(FiltroPromocion))
            Busqueda = FiltroPromocion;

        FiltrarProductos();
        IsBusy = false;
    }
}