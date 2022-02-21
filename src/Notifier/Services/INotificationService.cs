using EnvDTE;
using EnvDTE80;
using Notifier.Infrastructures;

namespace Notifier.Services
{
    internal interface INotificationService
    {
        void ClearHistory(DTE2 dte);
        void Show(DTE2 dte, vsBuildScope scope, string actionName, string timeFormat, BuildStatus status);
    }
}