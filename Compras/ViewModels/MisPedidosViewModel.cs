using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

public class MisPedidosViewModel : BaseViewModel
{
    private readonly OrdenesService _ordenesService;
    private readonly SesionService _sesionService;

    public ObservableCollection<OrdenDto> Ordenes { get; } = [];

    public MisPedidosViewModel(OrdenesService ordenesService, SesionService sesionService)
    {
        _ordenesService = ordenesService;
        _sesionService = sesionService;
        Titulo = "Mis Pedidos";

        VerRastreoCommand = new Command<OrdenDto>(async orden =>
            await Shell.Current.GoToAsync(
                "RastreoPage",
                new Dictionary<string, object> { ["IdOrden"] = orden.Id }));

        CargarCommand = new Command(async () => await CargarAsync());
        CargarCommand.Execute(null);
    }

    public Command CargarCommand { get; }
    public ICommand VerRastreoCommand { get; }

    private bool _sinPedidos;
    public bool SinPedidos
    {
        get => _sinPedidos;
        set { _sinPedidos = value; OnPropertyChanged(); }
    }

    private async Task CargarAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var ordenes = await _ordenesService.GetOrdenesPorUsuarioAsync(
            _sesionService.UsuarioActual?.Id ?? 0);

        Ordenes.Clear();
        foreach (var o in ordenes)
            Ordenes.Add(o);

        SinPedidos = !Ordenes.Any();
        IsBusy = false;
    }

    public async Task RecargarAsync()
    {
        await CargarAsync();
    }
}