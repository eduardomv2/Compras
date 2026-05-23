using Compras.ViewModels;

namespace Compras.Views;

public partial class CarritoPage : ContentPage
{
    public CarritoPage(CarritoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}