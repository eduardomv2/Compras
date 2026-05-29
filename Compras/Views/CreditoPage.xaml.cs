using Compras.ViewModels;

namespace Compras.Views;

public partial class CreditoPage : ContentPage
{
    private readonly CreditoViewModel _vm;

    public CreditoPage(CreditoViewModel vm)
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