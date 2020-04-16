using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Switchy.Model;

namespace Switchy.Wpf
{
    internal sealed class HighlightConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var match = value as Match<SwitchTarget>;
            if (match != null)
            {
                var input = match.Value.Name;
                var highlights = match.Ranges;
                var textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;

                var before = input.Substring(0,  highlights[1].Item1);
                textBlock.Inlines.Add(new Run(before));

                for(int i = 1; i<highlights.Count;i++)
                {
                    var hl = highlights[i];
                    var highlight = input.Substring(hl.Item1, hl.Item2);
                    textBlock.Inlines.Add(new Run(highlight){FontWeight = FontWeights.Bold, Background = Brushes.Red});
                    string after;
                    if (i < highlights.Count - 1)
                    {
                        var hlnext = highlights[i + 1];
                        after = input.Substring(hl.Item1 + hl.Item2, hlnext.Item1 - (hl.Item1 + hl.Item2));
                    }
                    else
                    {
                        after = input.Substring(hl.Item1 + hl.Item2, input.Length - (hl.Item1 + hl.Item2));
                    }
                    textBlock.Inlines.Add(new Run(after));
                }
                return textBlock;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }

    }
}
