using Compras.ViewModels;

namespace Compras.Views;

public partial class MisPedidosPage : ContentPage
{
    private readonly MisPedidosViewModel _vm;

    public MisPedidosPage(MisPedidosViewModel vm)
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