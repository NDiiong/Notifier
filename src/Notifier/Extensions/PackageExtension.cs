using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Notifier.Runtime;
using System;
using System.Reflection;
using System.Threading.Tasks;
using IInteropServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Notifier.Extensions
{
    public static class PackageExtension
    {
        public static void AddService<TImplement, TService>(this AsyncPackage package)
            where TImplement : class
            where TService : class, TImplement
        {
            package.AddService(typeof(TImplement), (_, __, ___) => CreateServiceAsync<TImplement, TService>(package, default), true);
        }

        public static void AddService<TImplement>(this AsyncPackage package, Func<IServiceProvider, TImplement> callback)
            where TImplement : class
        {
            package.AddService(typeof(TImplement), (_, __, ___) => CreateServiceAsync<TImplement, TImplement>(package, callback), true);
        }

        public static void AddService<TImplement, TService>(this AsyncPackage package, Func<IServiceProvider, TService> callback)
            where TImplement : class
            where TService : class, TImplement
        {
            package.AddService(typeof(TImplement), (_, __, ___) => CreateServiceAsync<TImplement, TService>(package, callback), true);
        }

        private async static Task<object> CreateServiceAsync<TImplement, TService>(AsyncPackage package, Func<IServiceProvider, TService> callback)
            where TImplement : class
            where TService : class
        {
            var dte = await package.GetServiceAsync<DTE, DTE2>();

            var serviceProvider = new ServiceProvider(dte.As<IInteropServiceProvider>());

            if (callback != null)
                return callback(serviceProvider);

            return await MapServiceContructorAsync<TService>(package, serviceProvider);
        }

        private static async Task<object> MapServiceContructorAsync<TService>(AsyncPackage package, ServiceProvider serviceProvider)
        {
            ConstructorInfo[] ctor = typeof(TService).GetConstructors();

            var parameters = ctor[0].GetParameters();
            var @params = Array.Empty<object>();

            foreach (var param in parameters)
            {
                if (param.ParameterType == typeof(IServiceProvider))
                    ArrayElement.Push(ref @params, serviceProvider);
                else
                {
                    var service = await package.GetServiceAsync(param.ParameterType, false);
                    ArrayElement.Push(ref @params, service);
                }
            }

            return Activator.CreateInstance(typeof(TService), @params);
        }
    }
}