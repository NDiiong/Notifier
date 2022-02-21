using Microsoft.VisualStudio.Shell;
using Notifier.Extensions;
using Notifier.Services;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Notifier
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("1217E191-B3FF-4537-9440-4B5007DF25A3")]
    [ProvideService(typeof(INotificationService), IsAsyncQueryable = true)]
    public class ServiceProviderPackage : AsyncPackage, IServiceProviderPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.AddService<INotificationService, ToastNotificationService>();
        }
    }
}