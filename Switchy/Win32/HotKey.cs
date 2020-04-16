using System;
using System.Windows.Input;
using System.Windows.Interop;

namespace Switchy.Win32
{
    internal sealed class HotKey : IDisposable
    {
        internal event Action<HotKey> HotKeyPressed;

        private readonly int _id;
        private bool _isKeyRegistered;
        readonly IntPtr _handle;

        internal HotKey(ModifierKeys modifierKeys, Key key, System.Windows.Window window)
            : this(modifierKeys, key, new WindowInteropHelper(window))
        {
        }

        internal HotKey(ModifierKeys modifierKeys, int key, System.Windows.Window window)
            : this(modifierKeys, key, new WindowInteropHelper(window))
        {
        }

        internal HotKey(ModifierKeys modifierKeys, Key key, WindowInteropHelper window)
            : this(modifierKeys, key, window.Handle)
        {
        }

        internal HotKey(ModifierKeys modifierKeys, int key, WindowInteropHelper window)
            : this(modifierKeys, key, window.Handle)
        {
        }

        internal HotKey(ModifierKeys modifierKeys, Key key, IntPtr windowHandle)
            : this(modifierKeys, KeyInterop.VirtualKeyFromKey(key), windowHandle)
        {
        }

        internal HotKey(ModifierKeys modifierKeys, int key, IntPtr windowHandle)
        {
            Key = key;
            KeyModifier = modifierKeys;
            _id = GetHashCode();
            _handle = windowHandle;
            RegisterHotKey();
            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
        }

        ~HotKey()
        {
            Dispose();
        }

        internal int Key { get; private set; }

        internal ModifierKeys KeyModifier { get; private set; }

        internal void RegisterHotKey()
        {
            if (Key == 0)
                return;
            if (_isKeyRegistered)
                UnregisterHotKey();
            _isKeyRegistered = HotKeyWinApi.RegisterHotKey(_handle, _id, KeyModifier, Key);
            if (!_isKeyRegistered)
                throw new ApplicationException("Hotkey already in use");
        }

        internal void UnregisterHotKey()
        {
            _isKeyRegistered = !HotKeyWinApi.UnregisterHotKey(_handle, _id);
        }

        public void Dispose()
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
            UnregisterHotKey();
        }

        private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == HotKeyWinApi.WmHotKey
                    && (int)(msg.wParam) == _id)
                {
                    OnHotKeyPressed();
                    handled = true;
                }
            }
        }

        private void OnHotKeyPressed()
        {
            if (HotKeyPressed != null)
                HotKeyPressed(this);
        }
    }
}
