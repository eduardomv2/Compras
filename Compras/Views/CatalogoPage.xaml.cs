using Compras.ViewModels;

namespace Compras.Views;

public partial class CatalogoPage : ContentPage
{
    private readonly CatalogoViewModel _vm;

    public CatalogoPage(CatalogoViewModel vm)
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