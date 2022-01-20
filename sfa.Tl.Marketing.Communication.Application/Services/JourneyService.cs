using System.Net;
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
            return $"{BaseUrl}origin={WebUtility.UrlEncode(fromPostcode)}&destination={WebUtility.UrlEncode(toLocation.Postcode)}&travelmode=transit";
        }

        public string GetDirectionsLink_2(string fromPostcode, string toPostcode)
        {
            //See https://developers.google.com/maps/documentation/urls/get-started#forming-the-directions-url

            return $"{BaseUrl}origin={WebUtility.UrlEncode(fromPostcode)}&destination={WebUtility.UrlEncode(toPostcode)}&travelmode=transit";
        }
    }
}
