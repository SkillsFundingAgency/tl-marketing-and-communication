
namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IJourneyService
    {
        string GetDirectionsLink(
            string fromPostcode, double fromLatitude, double fromLongitude, 
            string toPostcode, double toLatitude, double toLongitude);
    }
}
