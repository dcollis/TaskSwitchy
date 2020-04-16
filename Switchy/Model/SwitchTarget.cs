using System;
using System.Windows.Media;

namespace Switchy.Model
{
    internal abstract class SwitchTarget
    {
        private readonly string _name;

        internal SwitchTarget(String name)
        {
            _name = name;
        }

        public String Name { get { return _name; }}

        public ImageSource Icon { get; set; }

    }
}
