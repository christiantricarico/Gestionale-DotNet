using Gdn.Application;
using Gdn.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddApplication()
    .AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddPersistence(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("SqlServerDefault");
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options
            .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
            .EnableSensitiveDataLogging();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
