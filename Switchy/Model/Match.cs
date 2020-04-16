using System;
using System.Collections.Generic;

namespace Switchy.Model
{
    internal interface IMatch<out T>
    {
        T Value { get; }
        List<Tuple<int, int>> Ranges { get; } 
    }

    internal class Match<T> : IMatch<T>
    {
        private readonly T _value;
        private readonly List<Tuple<int, int>> _ranges;

        internal Match(T value, List<Tuple<int, int>> ranges)
        {
            _value = value;
            _ranges = ranges;
        }

        public T Value
        {
            get { return _value;}
        }
    

        public List<Tuple<int, int>> Ranges {
            get { return _ranges; }
        }
    }
}
