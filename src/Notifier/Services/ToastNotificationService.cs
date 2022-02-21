using EnvDTE;
using EnvDTE80;
using Microsoft.Toolkit.Uwp.Notifications;
using Notifier.Infrastructures;
using System;
using System.IO;
using Windows.UI.Notifications;
using Process = System.Diagnostics.Process;

namespace Notifier.Services
{
    internal class ToastNotificationService : INotificationService
    {
        private static readonly string _applicationID = "VisualStudio.";

        public void ClearHistory(DTE2 dte)
        {
            var applicationId = EditionToAppUserModelId(dte.Edition, dte.Version);
            ToastNotificationManager.History.Clear(applicationId);
        }

        public void Show(DTE2 dte, vsBuildScope scope, string actionName, string timeFormat, BuildStatus status)
        {
            ClearHistory(dte);

            var buildScope = BuildProjectMessage(scope);
            var installDirectory = GetExtensionInstallationDirectory();
            var basePath = Path.Combine(installDirectory, "Assets", $"{status}.png");
            var xmlDocument = new ToastContentBuilder()
                .AddAttributionText("Notifier")
                .AddArgument($"Vs.NotifierId-{Process.GetCurrentProcess().Id}")
                .SetToastDuration(ToastDuration.Short)
                .AddCustomTimeStamp(DateTime.Now)
                .AddText($"{actionName} {buildScope} {status.ToString().ToLower()}.")
                .AddText($"Elapsed: {timeFormat}")
                .AddAppLogoOverride(new Uri(basePath), ToastGenericAppLogoCrop.Default)
                .GetXml();

            var toastNotification = new ToastNotification(xmlDocument);
            var notifier = ToastNotificationManager.CreateToastNotifier(EditionToAppUserModelId(dte.Edition, dte.Version));
            notifier.Show(toastNotification);
        }

        private static string EditionToAppUserModelId(string edition, string version)
        {
            switch (edition)
            {
                case "WD Express":
                    return "VWDExpress." + version;

                case "Desktop Express":
                    return "WDExpress." + version;

                case "VSWin Express":
                    return "VSWinExpress." + version;

                case "PD Express":
                    return "VPDExpress." + version;
            }

            var s = Shell32dll.GetCurrentProcessExplicitAppUserModelID();
            return !string.IsNullOrEmpty(s) ? s : _applicationID + version;
        }

        private string BuildProjectMessage(vsBuildScope scope)
        {
            var buildScope = "project";

            if (scope == vsBuildScope.vsBuildScopeSolution)
                buildScope = "solution";

            return buildScope;
        }

        internal static string GetExtensionInstallationDirectory()
        {
            var uri = new Uri(typeof(NotifierPackage).Assembly.CodeBase, UriKind.Absolute);
            return Path.GetDirectoryName(uri.LocalPath);
        }
    }
}