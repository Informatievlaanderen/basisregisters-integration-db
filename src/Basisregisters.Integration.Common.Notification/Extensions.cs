// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

using Amazon.SimpleNotificationService;
using Basisregisters.Integration.Common.Notification;

public static class Extensions
{
    public static IServiceCollection AddNotificationService(this IServiceCollection services, string topicArn)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddScoped<INotificationService>(provider =>
        {
            var snsService = provider.GetRequiredService<IAmazonSimpleNotificationService>();
            return new NotificationService(snsService, topicArn);
        });

        return services;
    }
}
