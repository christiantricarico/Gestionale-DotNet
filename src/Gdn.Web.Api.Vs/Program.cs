using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();