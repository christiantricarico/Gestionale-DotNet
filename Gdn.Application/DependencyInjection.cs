using Microsoft.Extensions.DependencyInjection;

namespace Gdn.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
