using Microsoft.VisualStudio.Shell;
using System;

namespace Notifier.Services
{
    public interface IServiceProviderPackage : IServiceProvider, IAsyncServiceProvider
    {
    }
}