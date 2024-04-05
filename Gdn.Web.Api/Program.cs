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

string allowedOrigins = "gdn-client-apps";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:7199")
                            .AllowAnyHeader().AllowAnyMethod();

                          builder.WithOrigins("https://localhost:7088")
                            .AllowAnyHeader().AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(allowedOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
