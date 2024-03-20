using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7282/api/")
    });

//httpclient use example
//https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppCallWebApi

//builder.Services.AddHttpClient("WebAPI", client =>
//    client.BaseAddress = new Uri("https://localhost:7282/api/"));

await builder.Build().RunAsync();
