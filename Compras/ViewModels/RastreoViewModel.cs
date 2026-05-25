using Compras.Models;
using Compras.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Compras.ViewModels;

[QueryProperty(nameof(IdOrden), "IdOrden")]
public class RastreoViewModel : BaseViewModel
{
    private readonly EnviosService _enviosService;

    public ObservableCollection<RastreoDto> Historial { get; } = [];

    public string FechaEntregaMinima => Envio is null ? string.Empty
        : $"Llega a partir del {TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(Envio.FechaEstimada.AddDays(-2), DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City")):dd/MM/yyyy}";
    public string FechaEntregaMaxima => Envio is null ? string.Empty
        : $"A más tardar el {TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(Envio.FechaEstimada, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City")):dd/MM/yyyy}";

    public RastreoViewModel(EnviosService enviosService)
    {
        _enviosService = enviosService;
        Titulo = "Rastreo de pedido";
        RefreshCommand = new Command(async () => await CargarAsync());
    }

    public ICommand RefreshCommand { get; }

    private int _idOrden;
    public int IdOrden
    {
        get => _idOrden;
        set
        {
            _idOrden = value;
            OnPropertyChanged();
            _ = CargarAsync();
        }
    }

    private EnvioDto? _envio;
    public EnvioDto? Envio
    {
        get => _envio;
        set
        {
            _envio = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FechaEntregaMinima));
            OnPropertyChanged(nameof(FechaEntregaMaxima));
        }
    }

    private bool _sinEnvio;
    public bool SinEnvio
    {
        get => _sinEnvio;
        set { _sinEnvio = value; OnPropertyChanged(); }
    }

    private async Task CargarAsync()
    {
        if (IsBusy || _idOrden == 0) return;
        IsBusy = true;

        Envio = await _enviosService.GetEnvioPorOrdenAsync(_idOrden);
        SinEnvio = Envio is null;

        if (Envio is not null)
        {
            var historial = await _enviosService
                .GetHistorialRastreoAsync(Envio.Id);
            Historial.Clear();
            foreach (var h in historial)
                Historial.Add(h);
        }

        IsBusy = false;
    }
}