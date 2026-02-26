using Microsoft.Extensions.DependencyInjection;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.DataAccess._di;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<MijnQrCodesDbContext>();
        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

        return services;
    }
}
