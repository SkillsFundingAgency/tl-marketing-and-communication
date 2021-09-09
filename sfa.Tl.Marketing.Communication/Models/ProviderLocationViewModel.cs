using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace sfa.Tl.Marketing.Communication.Models
{
    [DebuggerDisplay("{" + nameof(ProviderName) + "}" +
                     " {" + nameof(Postcode) + ", nq}")]
    public class ProviderLocationViewModel
    {
        public string ProviderName { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DistanceInMiles { get; set; }
        public string DistanceString => DistanceInMiles == 1 ? "mile" : "miles";
        public IEnumerable<DeliveryYearViewModel> DeliveryYears { get; set; }
        public string Website { get; set; }
        public string RedirectUrl => !string.IsNullOrWhiteSpace(Website)
            ? $"/students/redirect?url={WebUtility.UrlEncode(Website)}"
            : "";

        public string RedirectUrlLabel => $"Visit {VenueName}'s website";

        public string VenueName => string.IsNullOrEmpty(Name) || Name == ProviderName
                    ? ProviderName
                    : Name;

        public string AddressLabel => !string.IsNullOrEmpty(Town)
            ? $"{Town} | {Postcode}"
            : $"{Postcode}";

        public bool HasFocus { get; set; }

        public string JourneyUrl { get; set; }
    }
}
