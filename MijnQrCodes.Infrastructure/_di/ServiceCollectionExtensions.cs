using Microsoft.Extensions.DependencyInjection;

namespace MijnQrCodes.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<QrCodeDbContext>();

        return services;
    }
}