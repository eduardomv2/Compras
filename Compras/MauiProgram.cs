using System;
using System.Net.Http;
using Compras.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Compras.ViewModels;
using Compras.Views;


namespace Compras;

public static class MauiProgram
{
    // URL del Gateway — cambiar cuando estén en red local
    //public const string GatewayUrl = "https:/localhost:7007";
    public const string GatewayUrl = "http://10.0.2.2:5133";

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── HttpClient apuntando al Gateway ──────────────────────
        builder.Services.AddHttpClient("Gateway", client =>
        {
            client.BaseAddress = new Uri(GatewayUrl);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // Ignorar certificado SSL en desarrollo
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        // Services
        builder.Services.AddSingleton<SesionService>();
        builder.Services.AddSingleton<CarritoService>();
        builder.Services.AddTransient<CatalogoService>();
        builder.Services.AddTransient<ClientesService>();
        builder.Services.AddTransient<PagosService>();
        builder.Services.AddTransient<OrdenesService>();
        builder.Services.AddTransient<CheckoutViewModel>();

        // ViewModels 
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CatalogoViewModel>();
        builder.Services.AddTransient<CarritoViewModel>();
        builder.Services.AddTransient<ProductoDetalleViewModel>();
        builder.Services.AddTransient<RegistroViewModel>();
        builder.Services.AddTransient<CheckoutViewModel>();
        builder.Services.AddTransient<MisPedidosViewModel>();


        // Vistas
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CatalogoPage>();
        builder.Services.AddTransient<CarritoPage>();
        builder.Services.AddTransient<ProductoDetallePage>();
        builder.Services.AddTransient<RegistroPage>();          
        builder.Services.AddTransient<CheckoutPage>();    
        builder.Services.AddTransient<MisPedidosPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}