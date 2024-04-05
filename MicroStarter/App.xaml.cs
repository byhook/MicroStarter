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
        Mutex mutex = new Mutex(true, "MicroStarter");
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