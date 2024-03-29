﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Notifier.Commands;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace Notifier
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(NotifierPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class NotifierPackage : AsyncPackage
    {
        public const string PackageGuidString = "ebc84a8f-c8d1-4034-a9e3-b5e0d4a2cefb";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await BuildStatusCommand.InitializeAsync(this).ConfigureAwait(false);
        }
    }
}