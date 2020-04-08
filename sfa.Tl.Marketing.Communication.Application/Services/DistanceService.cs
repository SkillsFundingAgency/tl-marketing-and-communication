using sfa.Tl.Marketing.Communication.Application.Haversine;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class DistanceService : IDistanceService
    {
        public double CalculateInMiles(double lat1, double lon1, double lat2, double lon2)
        {
            Position pos1 = new Position();
            pos1.Latitude = lat1;
            pos1.Longitude = lon1;

            Position pos2 = new Position();
            pos2.Latitude = lat2;
            pos2.Longitude = lon2;

            var distanceInMiles = Haversine.Haversine.Distance(pos1, pos2, DistanceType.Miles);

            return distanceInMiles;
        }
    }
}
