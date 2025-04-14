using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddPersistence(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("SqlServerDefault");
    options.UseSqlServer(connectionString);

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
    .AddConfigOptions(builder.Configuration)
    .AddReports()
    .AddFatturaElettronica();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.MapEndpoints();

//app.UseExceptionHandler(); // Converts unhandled exceptions into Problem Details responses
//app.UseStatusCodePages(); // Returns the Problem Details response for (empty) non-successful responses

QuestPDF.Settings.License = LicenseType.Community;

app.Run();