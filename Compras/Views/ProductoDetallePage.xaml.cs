using Compras.ViewModels;

namespace Compras.Views;

public partial class ProductoDetallePage : ContentPage
{
    public ProductoDetallePage(ProductoDetalleViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}