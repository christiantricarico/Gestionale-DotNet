using Gdn.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gdn.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbContextOptions)
    {
        services.AddDbContext<AppDbContext>(dbContextOptions);

        AddRepositories(services);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        IEnumerable<Type> repoTypes = types.Where(t => t.IsClass == true && t.IsAbstract == false && t.Name.EndsWith("Repository"));

        foreach (Type repoType in repoTypes)
        {
            Type? interfaceType = repoType.GetInterface($"I{repoType.Name}");
            if (interfaceType is not null)
                services.AddScoped(interfaceType, repoType);
        }

        return services;
    }
}
