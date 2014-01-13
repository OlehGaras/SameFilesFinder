using System.Collections.Generic;
using System.Windows;
using SameFileFinder;

namespace WpfSameFileFinder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow(new MainWindowViewModel());
            window.Show();
        }
    }
}
