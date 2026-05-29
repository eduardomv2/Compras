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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.RecargarAsync();
    }
}