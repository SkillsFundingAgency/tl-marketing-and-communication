using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.GoogleMaps
{
    public interface IGoogleMapApiClient
    {
        Task<string> GetAddressDetailsAsync(string postcode);

        Task<int> ComputeDistanceBetweenInMiles(double lat1, double lon1, double lat2, double lon2);
    }
}