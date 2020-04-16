using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Switchy.Model;

namespace Switchy.Match
{
    internal class IdeaLikeItemMatcher<T> : IItemMatcher<T> {
   
        public IEnumerable<Match<T>> Match(List<T> values, Func<T, String> extracter, String input) {
            var pattern = Patternize(input);
            return values.Select(t => Match(t, extracter, pattern)).Where(t => t != null);
        }

    private static Match<T> Match(T value, Func<T, String> extracter, Regex pattern)
    {
        var match = pattern.Match(extracter(value));
        if (match.Success)
        {
             var ranges = new List<Tuple<int, int>>();
            foreach (Group group in match.Groups)
            {
                ranges.Add(new Tuple<int,int>(group.Index, group.Length));
            }
            return new Match<T>(value, ranges);
        }
        return null;
    }

    static Regex Patternize(string input) {
        var tokens = Tokenize(input);
        var builder = new StringBuilder("(?i).*?");
        foreach (string token in tokens) {
            builder.Append("(").Append(token).Append(")").Append(".*?");
        }
        return new Regex(builder.ToString());
    }

    static IEnumerable<string> Tokenize(string input) {
        var ret = new List<string>();
        var builder = new StringBuilder();
        foreach (char c in input) {
            if (Char.IsUpper(c) || !(Char.IsLetter(c) || Char.IsDigit(c))) {
                ret.Add(builder.ToString());
                builder.Clear();
            }
            builder.Append(c);
        }
        ret.Add(builder.ToString());
        return ret.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
    }

    internal static String Quote(String s) {
        int slashEIndex = s.IndexOf("\\E", StringComparison.InvariantCulture);
        if (slashEIndex == -1)
            return "\\Q" + s + "\\E";

        var sb = new StringBuilder(s.Length * 2);
        sb.Append("\\Q");
        int current = 0;
        while ((slashEIndex = s.IndexOf("\\E", current, StringComparison.InvariantCulture)) != -1) {
            sb.Append(s.Substring(current, (slashEIndex - current) +1));
            current = slashEIndex + 2;
            sb.Append("\\E\\\\E\\Q");
        }
        sb.Append(s.Substring(current, (s.Length - current) +1));
        sb.Append("\\E");
        return sb.ToString();
    }
}

}
