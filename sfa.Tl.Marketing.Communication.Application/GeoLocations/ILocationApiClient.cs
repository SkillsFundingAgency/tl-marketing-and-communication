using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.GeoLocations
{
    public interface ILocationApiClient
    {
        //Task<(bool, string)> IsValidPostcodeAsync(string postcode);
        Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode);
    }
}