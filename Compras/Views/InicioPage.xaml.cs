using Compras.ViewModels;

namespace Compras.Views;

public partial class InicioPage : ContentPage
{
    private readonly InicioViewModel _vm;

    public InicioPage(InicioViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(InicioViewModel.PromocionActual))
                carruselPromociones.ScrollTo(_vm.PromocionActual);
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.RecargarAsync();
    }
}