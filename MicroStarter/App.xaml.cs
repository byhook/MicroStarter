using System.Configuration;
using System.Data;
using System.Windows;

namespace MicroStarter;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        var mutexName = "Debug";
#else
        var  mutexName = "Release";
#endif
        Mutex mutex = new Mutex(true, "MicroStarter" + mutexName);
        if (mutex.WaitOne(0, false))
        {
            base.OnStartup(e);
        }
        else
        {
            this.Shutdown();
        }
    }
}