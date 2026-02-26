using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MijnQrCodes.Application.Services;
using MijnQrCodes.DataAccess._di;

namespace MijnQrCodes.Application._di;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddDataAccess();

        return services;
    }
}
