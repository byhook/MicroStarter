using System.Configuration;
using System.Data;
using System.Windows;

namespace MicroStarter;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    
    private static Mutex mutex;
    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        var mutexName = "Debug";
#else
        var  mutexName = "Release";
#endif
        bool createdNew;
        mutex = new Mutex(true, "MicroStarter" + mutexName,out createdNew);
        if (createdNew)
        {
            base.OnStartup(e);
        }
        else
        {
            this.Shutdown();
        }
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        mutex.ReleaseMutex();
        base.OnExit(e);
    }
    
}