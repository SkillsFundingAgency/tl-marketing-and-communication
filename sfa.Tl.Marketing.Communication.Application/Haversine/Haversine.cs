using System;
using sfa.Tl.Marketing.Communication.Application.Enums;

namespace sfa.Tl.Marketing.Communication.Application.Haversine
{
    /// <summary>
    /// Haversine distance calculation.
    /// code reference
    /// https://gist.github.com/jammin77/033a332542aa24889452
    /// </summary>
    public static class Haversine
    {
        /// <summary>
        /// Returns the distance in miles or kilometers of any two
        /// latitude / longitude points.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static double Distance(Position pos1, Position pos2, DistanceType type)
        {
            // ReSharper disable once InconsistentNaming
            var R = (type == DistanceType.Miles) ? 3960 : 6371;
            var dLat = ToRadians(pos2.Latitude - pos1.Latitude);
            var dLon = ToRadians(pos2.Longitude - pos1.Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(pos1.Latitude)) * Math.Cos(ToRadians(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            var d = R * c;
            return d;
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
