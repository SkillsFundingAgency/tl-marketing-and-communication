using System.Text;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class JourneyService : IJourneyService
    {
        private const string BaseUrl = "https://www.google.com/maps/dir/?api=1&";

        public string GetDirectionsLink(
            string fromPostcode, double fromLatitude, double fromLongitude, 
            string toPostcode, double toLatitude, double toLongitude)
        {
        //See https://developers.google.com/maps/documentation/urls/get-started#forming-the-directions-url

        //return "https://www.google.com/maps/dir/B91+1NG,+Solihull/Solihull+College+%26+University+Centre+Blossomfield+Campus,+Solihull/@52.4113588,-1.795049,17z/data=!3m1!4b1!4m14!4m13!1m5!1m1!1s0x4870b9e9b63f91ef:0x2a0e6b03104f3776!2m2!1d-1.7921997!2d52.4131839!1m5!1m1!1s0x4870b9c249edef4d:0xa9adeba9e68f74c!2m2!1d-1.7924987!2d52.4092323!3e3";



        // https://www.google.com/maps/dir/?api=1&origin=CV1+2WT&destination=52.409568,-1.792148&travelmode=transit

            var uriBuilder = new StringBuilder(BaseUrl);

            //https://stackoverflow.com/questions/45116011/generate-a-google-map-link-with-directions-using-latitude-and-longitude

            //https://www.google.com/maps/dir/B91+1NG,+Solihull/Solihull+College+%26+University+Centre+Blossomfield+Campus,+Solihull/@52.4113588,-1.795049,17z/data=!3m1!4b1!4m14!4m13!1m5!1m1!1s0x4870b9e9b63f91ef:0x2a0e6b03104f3776!2m2!1d-1.7921997!2d52.4131839!1m5!1m1!1s0x4870b9c249edef4d:0xa9adeba9e68f74c!2m2!1d-1.7924987!2d52.4092323!3e3

            //OX2 9GX to 
            //OX1 1SA "Activate Learning" City of Oxford College
            // https://www.google.com/maps/dir/?api=1&origin=OX2+9GX&destination=OX1+1SA,Activate+Learning,City+of+Oxford+College&travelmode=transit

            //var latDes = this.items[id].LongitudeWD;
            //var longDes = this.items[id].LatitudeWD;

            //uriBuilder.Append("&map_action=map");
            //TODO: Use postcodes/address?
            uriBuilder.Append($"origin={fromLatitude},{fromLongitude}");
            uriBuilder.Append($"&destination={toLatitude},{toLongitude}");
            //uriBuilder.Append("&region=uk");
            //TODO: try without this
            uriBuilder.Append("&travelmode=transit");
            //uriBuilder.Append("&layer=transit");

            return uriBuilder.ToString();
        }
    }
}
