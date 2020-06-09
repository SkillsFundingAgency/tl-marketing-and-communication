using System;

namespace sfa.Tl.Marketing.Communication.Application.Haversine
{
    /// <summary>
    /// The distance type to return the results in.
    /// code reference
    /// https://gist.github.com/jammin77/033a332542aa24889452
    /// </summary>
    public enum DistanceType { Miles, Kilometers };
    
    /// <summary>
    /// Specifies a Latitude / Longitude point.
    /// </summary>
    public struct Position
    {
        public double Latitude;
        public double Longitude;
    }

    public static class Haversine
    {
        /// <summary>
        /// Returns the distance in miles or kilometers of any two
        /// latitude / longitude points.
        /// </summary>
        /// <param name=”pos1″></param>
        /// <param name=”pos2″></param>
        /// <param name=”type”></param>
        /// <returns></returns>
        public static double Distance(Position pos1, Position pos2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = toRadian(pos2.Latitude-pos1.Latitude);
            double dLon = toRadian(pos2.Longitude-pos1.Longitude);
                       
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(toRadian(pos1.Latitude)) * Math.Cos(toRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name=”val”></param>
        /// <returns></returns>
        private static double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
