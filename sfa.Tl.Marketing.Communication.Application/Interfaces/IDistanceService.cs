namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IDistanceService
    {
        double CalculateInMiles(double lat1, double lon1, double lat2, double lon2);
    }
}
