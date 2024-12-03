using Microsoft.Extensions.DependencyInjection;
using MijnQrCodes.Infrastructure;
using System.Reflection;

namespace MijnQrCodes.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddInfrastructure();

        return services;
    }
}