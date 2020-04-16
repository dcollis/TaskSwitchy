using System.Windows.Input;
using Switchy.Win32;

namespace Switchy
{
    internal sealed class MainWindowController
    {
        private readonly MainWindow _window;
        private readonly MainWindowViewModel _viewModel;
        

        internal MainWindowController()
        {
            _viewModel = new MainWindowViewModel();
            _window = new MainWindow(_viewModel);
            var hotKey = new HotKey(ModifierKeys.Alt, 192, _window);
            hotKey.HotKeyPressed += HotKeyHotKeyPressed;
        }

        private void HotKeyHotKeyPressed(HotKey obj)
        {
            _window.Show();
            _viewModel.Init(_window.WindowHandle);
            WindowManagerApi.ShowWindow(_window.WindowHandle);
        }
    }
}