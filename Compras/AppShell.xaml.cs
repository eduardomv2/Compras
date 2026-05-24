namespace Compras;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("RegistroPage", typeof(Views.RegistroPage));
        Routing.RegisterRoute("ProductoDetallePage", typeof(Views.ProductoDetallePage));
        Routing.RegisterRoute("CheckoutPage", typeof(Views.CheckoutPage));
    }
}