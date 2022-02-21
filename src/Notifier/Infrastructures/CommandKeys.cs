namespace Notifier.Infrastructures
{
    public static class CommandKeys
    {
        public static class Build
        {
            public const string CancelBuild = "Build.Cancel";
        }

        public static class View
        {
            public const string ViewErrorList = "View.ErrorList";
        }
    }

    internal enum BuildStatus
    {
        Failed,
        Succeeded
    }
}