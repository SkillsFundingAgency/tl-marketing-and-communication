using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Models.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ChunkString(this string str, int chunkLength)
        {
            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }

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
                "a", "an", "and", "any", "at", "for", "from", "into", 
                "of", "on", "or", "some", "the", "to",
            };

            result = tokens[0];
            tokens.RemoveAt(0);
            
            result += tokens.Aggregate(string.Empty, (prev, input)
                => prev +
                   (artsAndPreps.Contains(input.ToLower()) 
                       ? " " + input.ToLower()              // Return the prep/art lowercase
                       : " " + input));                     // Otherwise return the valid word

            return result.Trim();
        }
    }
}