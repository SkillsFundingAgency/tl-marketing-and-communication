using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace sfa.Tl.Marketing.Communication.DataLoad.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var artsAndPreps = new List<string>
            { "a", "an", "and", "any", "at", "for", "from", "into", "of", "on",
                "or", "some", "the", "to", };

            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());

            var tokens = result.Split(new[] { ' ', '\t', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            result = tokens[0];
            tokens.RemoveAt(0);

            result += tokens.Aggregate(string.Empty, (prev, input)
                => prev +
                   (artsAndPreps.Contains(input.ToLower()) // If True
                       ? " " + input.ToLower()              // Return the prep/art lowercase
                       : " " + input));                   // Otherwise return the valid word.

            result = Regex.Replace(result, @"(?!^Out)(Out\s+Of)", "out of");

            return result;
        }
    }
}
