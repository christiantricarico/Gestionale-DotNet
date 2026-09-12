using Gdn.Web.MudBlazor;
using Gdn.Web.MudBlazor.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var backendUri = new Uri(builder.Configuration["BackendUrl"] ?? builder.HostEnvironment.BaseAddress);

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new TokenStore(sp.GetRequiredService<IJSRuntime>()));

// A bare client for the authentication endpoints, deliberately without the handler below, so
// renewing a token cannot end up calling the handler that asked for it.
builder.Services.AddScoped(sp => new AuthClient(new HttpClient { BaseAddress = backendUri }));

builder.Services.AddScoped<GdnAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<GdnAuthenticationStateProvider>());

// The application wide client keeps the same base address and relative paths as before, so every
// existing page keeps working unchanged while gaining the bearer token.
builder.Services.AddScoped(sp =>
{
    var handler = new AuthorizationMessageHandler(sp.GetRequiredService<GdnAuthenticationStateProvider>())
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(handler) { BaseAddress = backendUri };
});

builder.Services.AddMudServices();

await builder.Build().RunAsync();
