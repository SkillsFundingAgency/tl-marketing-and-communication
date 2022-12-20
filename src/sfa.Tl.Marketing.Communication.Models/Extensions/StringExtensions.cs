using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace sfa.Tl.Marketing.Communication.Models.Extensions;

public static class StringExtensions
{
    private const string PostcodeRegex =
        @"(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX‌​]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]))))\s?[0-9][A-Z-[C‌​IKMOV]]{2})(\w)*$";

    private const string PartialPostcodeRegex = @"((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY])))))$";

    public static bool IsPostcode(this string postcode)
    {
        return CheckPostcode(postcode, PostcodeRegex);
    }

    public static bool IsPartialPostcode(this string postcode)
    {
        return CheckPostcode(postcode, PartialPostcodeRegex);
    }

    public static bool IsFullOrPartialPostcode(this string postcode)
    {
        return postcode.IsPostcode() || postcode.IsPartialPostcode();
    }

    private static bool CheckPostcode(string postcode, string regex)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return false;

        var formattedPostcode = postcode.Trim().ToUpperInvariant();

        return Regex.IsMatch(formattedPostcode, regex);
    }

    public static string ToLetterOrDigit(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty :
            new string(Array.FindAll(value.ToCharArray(), char.IsLetterOrDigit));
    }

    public static string ToSearchableString(this string value)
    {
        if (value == null)
            return null;

        //Remove special characters and spaces, and replace & with and
        const string knownSpecialCharacters = @"(\s+|,|\.|'|\-|!|\(|\)|/)";
        return Regex.Replace(
                Regex.Replace(value, knownSpecialCharacters, ""),
                @"(&)", "and")
            .ToLower();
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