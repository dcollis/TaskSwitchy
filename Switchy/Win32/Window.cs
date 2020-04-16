using System;
using Switchy.Model;

namespace Switchy.Win32
{
    internal class Window : SwitchTarget
    {
        private readonly IntPtr _windowPointer;
        internal Window(string name, IntPtr windowPointer) : base(name)
        {
            _windowPointer = windowPointer;
        }

        internal IntPtr WindowPointer { get { return _windowPointer; } }

        
    }
}
