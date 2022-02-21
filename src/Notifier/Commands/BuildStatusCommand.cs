using EnvDTE;
using EnvDTE80;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.VisualStudio.Shell;
using Notifier.Extensions;
using Notifier.Infrastructures;
using Notifier.Services;
using System;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace Notifier.Commands
{
    internal static class BuildStatusCommand
    {
        private static bool _failed;
        private static DateTime _startTime;
        private static BuildEvents _buildEvents;
        private static INotificationService _notificationService;

        internal static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var dte = await package.GetServiceAsync<DTE, DTE2>();
            _notificationService = package.GetService<INotificationService>();

            _buildEvents = dte.Events.BuildEvents;
            _buildEvents.OnBuildBegin += (_, __) => OnBuildBegin();
            _buildEvents.OnBuildDone += (scope, action) => OnBuildDone(dte, scope, action);
            _buildEvents.OnBuildProjConfigDone += (_, __, ___, ____, success) => BuildConfigDone(dte, success);

            ToastNotificationManagerCompat.OnActivated += OnActivated;
        }

        private static void OnActivated(ToastNotificationActivatedEventArgsCompat args)
        {
            if (!args.Argument.StartsWith("Vs.NotifierId-"))
                return;

            var processID = int.Parse(args.Argument.Split('-')[1]);
            var VSProcess = Process.GetProcessById(processID);
            if (VSProcess != null)
            {
                var VSIntPtr = VSProcess.MainWindowHandle;

                //get the hWnd of the process
                var placement = new User32dll.Windowplacement();
                User32dll.GetWindowPlacement(VSIntPtr, ref placement);

                // Check if window is minimized
                if (placement.showCmd == 2)
                {
                    //the window is hidden so we restore it
                    User32dll.ShowWindow(VSIntPtr, 9);
                }

                //set user's focus to the window
                User32dll.SetForegroundWindow(VSIntPtr);
            }
        }

        private static void OnBuildBegin()
        {
            _startTime = DateTime.Now;
            _failed = false;
        }

        private static void BuildConfigDone(DTE2 dte, bool success)
        {
            _failed = !success;

            if (success)
                return;

            dte.ExecuteCommand(CommandKeys.Build.CancelBuild);
            dte.ExecuteCommand(CommandKeys.View.ViewErrorList);
        }

        private static void OnBuildDone(DTE2 dte, vsBuildScope scope, vsBuildAction action)
        {
            var time = DateTime.Now - _startTime;
            var actionName = ActionName(action);
            var timeFormat = TimeFormat(time);

            var status = _failed ? BuildStatus.Failed : BuildStatus.Succeeded;
            _notificationService.Show(dte, scope, actionName, timeFormat, status);
        }

        private static string ActionName(vsBuildAction action)
        {
            switch (action)
            {
                case vsBuildAction.vsBuildActionBuild:
                    return "Build";

                case vsBuildAction.vsBuildActionRebuildAll:
                    return "Rebuild";

                case vsBuildAction.vsBuildActionClean:
                    return "Clean";

                case vsBuildAction.vsBuildActionDeploy:
                    return "Deploy";
            }

            return "Build";
        }

        private static string TimeFormat(TimeSpan time)
        {
            if (time.Minutes == 0)
                return $"{time:s\\.fff} seconds";

            if (time.Hours == 0)
                return $"{time:mm\\:ss\\.fff} minutes";

            return string.Format("{0:hh\\:m\\:ss\\.fff}", time);
        }
    }
}