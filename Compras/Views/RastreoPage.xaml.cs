using Compras.ViewModels;

namespace Compras.Views;

public partial class RastreoPage : ContentPage
{
    public RastreoPage(RastreoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}