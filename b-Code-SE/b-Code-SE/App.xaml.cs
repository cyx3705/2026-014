using System.Windows;
using b_Code_SE.Interop;

namespace b_Code_SE;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        OleMessageFilter.Register();
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        OleMessageFilter.Revoke();
        base.OnExit(e);
    }
}