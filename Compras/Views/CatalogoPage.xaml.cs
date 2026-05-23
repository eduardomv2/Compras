using Compras.ViewModels;

namespace Compras.Views;

public partial class CatalogoPage : ContentPage
{
    public CatalogoPage(CatalogoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}