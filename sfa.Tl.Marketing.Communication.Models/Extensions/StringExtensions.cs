using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace sfa.Tl.Marketing.Communication.Models.Extensions;

public static class StringExtensions
{
    public static string ToLetterOrDigit(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty :
            new string(Array.FindAll(value.ToCharArray(), char.IsLetterOrDigit));
    }

    public static string ToTitleCase(this string value)
    {
        if (value == null)
            return null;

        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());

        var tokens = result.Split(new[] { ' ', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        var artsAndPreps = new List<string>
        {
            "a", "an", "and", "any", "at", "for", "from", "in", "into",
            "of", "on", "or", "some", "the", "to"
        };

        result = tokens[0];
        tokens.RemoveAt(0);

        result += tokens.Aggregate(string.Empty, (prev, input)
            => prev +
               (artsAndPreps.Contains(input.ToLower())
                   ? " " + input.ToLower() // Return the prep/art lowercase
                   : " " + input));        // Otherwise return the valid word

        //Fix S after apostrophe, if it is before space or at end of string
        result = Regex.Replace(result, @"(['’])S(\s|$)", "$1s$2");

        return result.Trim();
    }
}