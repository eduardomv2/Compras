using Compras.ViewModels;

namespace Compras.Views;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(CheckoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}