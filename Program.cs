using System.Diagnostics;
using System.Security.Principal;
using TrayCommander;
using TrayCommander.Services;

Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

static bool IsElevated()
{
    using var identity = WindowsIdentity.GetCurrent();
    return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
}

var options = PersistenceService.LoadOptions();
var isElevated = IsElevated();

if (options.AlwaysRunAsAdmin && !isElevated)
{
    var exe = Process.GetCurrentProcess().MainModule?.FileName;
    if (exe != null)
    {
        Process.Start(new ProcessStartInfo(exe) { Verb = "runas", UseShellExecute = true });
        return;
    }
}

Application.Run(new TrayApp(isElevated));
