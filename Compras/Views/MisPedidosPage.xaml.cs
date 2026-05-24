using Compras.ViewModels;

namespace Compras.Views;

public partial class MisPedidosPage : ContentPage
{
    public MisPedidosPage(MisPedidosViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}