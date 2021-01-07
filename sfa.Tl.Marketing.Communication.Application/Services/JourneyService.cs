using System.Net;
using System.Text;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class JourneyService : IJourneyService
    {
        private const string BaseUrl = "https://www.google.com/maps/dir/?api=1&";

        public string GetDirectionsLink(string fromPostcode, ProviderLocation toLocation)
        {
            //See https://developers.google.com/maps/documentation/urls/get-started#forming-the-directions-url

            var uriBuilder = new StringBuilder(BaseUrl);
            uriBuilder.Append($"origin={WebUtility.UrlEncode(fromPostcode)}");
            uriBuilder.Append($"&destination={WebUtility.UrlEncode(toLocation.Postcode)}");
            uriBuilder.Append($",{WebUtility.UrlEncode(toLocation.ProviderName)}");
            if (!string.IsNullOrWhiteSpace(toLocation.Name))
            {
                uriBuilder.Append($",{WebUtility.UrlEncode(toLocation.Name)}");
            }

            // ReSharper disable once StringLiteralTypo
            uriBuilder.Append("&travelmode=transit");

            return uriBuilder.ToString();
        }
    }
}
