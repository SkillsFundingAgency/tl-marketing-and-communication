using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IJourneyService
    {
        string GetDirectionsLink(string fromPostcode, ProviderLocation toLocation);
    }
}
