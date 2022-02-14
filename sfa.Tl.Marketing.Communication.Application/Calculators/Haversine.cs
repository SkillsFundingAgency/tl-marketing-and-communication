using System;

namespace sfa.Tl.Marketing.Communication.Application.Calculators
{
    /// <summary>
    /// Haversine distance calculation.
    /// Code reference: https://gist.github.com/jammin77/033a332542aa24889452
    /// </summary>
    public static class Haversine
    {
        private const int R = 3960;
        private const double DegreesToRadians = Math.PI / 180;
        
        public static double Distance(double pos1Latitude, double pos1Longitude, double pos2Latitude, double pos2Longitude)
        {
            var dLat = (pos2Latitude - pos1Latitude) * DegreesToRadians;
            var dLon = (pos2Longitude - pos1Longitude) * DegreesToRadians;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(pos1Latitude * DegreesToRadians) * 
                Math.Cos(pos2Latitude * DegreesToRadians) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return R * c;
        }
    }
}
