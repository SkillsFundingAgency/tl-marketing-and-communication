using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.GeoLocations
{
    public interface ILocationApiClient
    {
        Task<(bool, string)> IsValidPostcodeAsync(string postcode);
        Task<(bool, string)> IsValidPostcodeAsync(string postcode, bool includeTerminated);
        Task<(bool, string)> IsTerminatedPostcodeAsync(string postcode);
        Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode);
        Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode, bool includeTerminated);
        Task<PostcodeLookupResultDto> GetTerminatedPostcodeGeoLocationDataAsync(string postcode);
    }
}