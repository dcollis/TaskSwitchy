using System;
using System.Collections.Generic;

namespace Switchy.Model
{
    internal interface IItemMatcher<T> {
        IEnumerable<Match<T>> Match(List<T> values, Func<T, string> extracter, string input);
    }

}
