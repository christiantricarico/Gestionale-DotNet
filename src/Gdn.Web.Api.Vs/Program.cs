using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using TinyHelpers.AspNetCore.Extensions;

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

// from tinyhelpers lib -> by default add detail, instance, traceid, stacktrace to problem details response
// https://www.youtube.com/watch?v=anqV3zkeyrM
builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler(); // Converts unhandled exceptions into Problem Details responses in production environment
app.UseStatusCodePages(); // Returns the Problem Details response for (empty) non-successful responses

app.MapEndpoints();

QuestPDF.Settings.License = LicenseType.Community;

app.Run();