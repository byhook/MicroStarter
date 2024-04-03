using System.Windows;
using System.Windows.Markup;

namespace MicroStarter;


public class XamlLoader
{
    public static T LoadXaml<T>(string xaml) where T : FrameworkElement
    {
        T result = (T)XamlReader.Parse(xaml);
        return result;
    }
}