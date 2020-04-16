using System.Windows;

namespace Switchy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // ReSharper disable once NotAccessedField.Local
        private MainWindowController _controller;

        protected override void OnStartup(StartupEventArgs e)
        {
            _controller = new MainWindowController();
            base.OnStartup(e);
        }
    }
}
