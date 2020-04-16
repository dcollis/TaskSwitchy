using System;
using System.Windows;
using System.Windows.Interop;
using Switchy.Win32;

namespace Switchy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal sealed partial class MainWindow
    {

        private IntPtr _windowHandle;
        private IntPtr _thumbHandle;

        internal IntPtr WindowHandle
        {
            get
            {
                if (_windowHandle == IntPtr.Zero) _windowHandle = (new WindowInteropHelper(this)).Handle;
                return _windowHandle;
            }
        }

        private MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel) DataContext; }
        }

        internal MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            Activated += MainWindowActivated;
            LostFocus += MainWindowLostFocus;
            Deactivated += MainWindow_Deactivated;
            viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Preview") ClearAnyDwmPreview();
            if (e.PropertyName == "SelectedMatch" && ViewModel.Preview == null && ViewModel.SelectedMatch != null) TrySetPreviewUsingDwm();
        }

        private void ClearAnyDwmPreview()
        {
            DesktopWindowManagerApi.HideThumb(_thumbHandle);
            _thumbHandle = IntPtr.Zero;
        }

        private void TrySetPreviewUsingDwm()
        {
            ClearAnyDwmPreview();
            _thumbHandle = DesktopWindowManagerApi.ShowThumb(WindowHandle, ViewModel.SelectedMatch.Value as Win32.Window,  new DesktopWindowManagerApi.Rect(500, 0, 800, 200));
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            ClearAnyDwmPreview();
            Hide();
        }

        private void MainWindowLostFocus(object sender, RoutedEventArgs e)
        {
            ClearAnyDwmPreview();
            Hide();
        }

        private void MainWindowActivated(object sender, EventArgs e)
        {
            SearchBox.Focus();
        }

    }
}
