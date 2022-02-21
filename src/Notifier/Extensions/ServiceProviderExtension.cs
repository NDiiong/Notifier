using System;

namespace Notifier.Extensions
{
    public static class ServiceProviderExtension
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }
    }
}