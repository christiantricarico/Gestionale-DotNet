using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddPersistence(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("SqlConnection");
    options.UseAzureSql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        //options
        //    .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
        //    .EnableSensitiveDataLogging();
    }
});

builder.Services.AddEndpoints();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services
    .AddReports()
    .AddFatturaElettronica();

var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (appSettings.AllowedOrigins?.Length > 0)
            policy.WithOrigins(appSettings.AllowedOrigins);
        else
            policy.SetIsOriginAllowed(_ => false);

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

// from tinyhelpers lib -> by default add detail, instance, traceid, stacktrace to problem details response
// https://www.youtube.com/watch?v=anqV3zkeyrM
builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();
builder.Services.AddRequestLocalization(appSettings.SupportedCultures ?? ["it-IT"]);

builder.Services.AddOpenApi(options =>
{
    options.AddAcceptLanguageHeader();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

await UpdateDatabaseAsync(app.Services);

// Configure the HTTP request pipeline explicitly
app.UseExceptionHandler(); // Converts unhandled exceptions into Problem Details responses in production environment
app.UseStatusCodePages(); // Returns the Problem Details response for (empty) non-successful responses
app.UseHttpsRedirection();
app.UseCors();
app.UseRequestLocalization();
app.UseRouting();

app.MapDefaultEndpoints();
app.MapEndpoints();

QuestPDF.Settings.License = LicenseType.Community;

app.Run();

static async Task UpdateDatabaseAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}