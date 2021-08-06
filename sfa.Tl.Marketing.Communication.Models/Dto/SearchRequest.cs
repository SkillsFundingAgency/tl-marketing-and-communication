namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class SearchRequest
    {
        public string Postcode { get; init; }
        public string OriginLatitude { get; init; }
        public string OriginLongitude { get; init; }
        public int NumberOfItems { get; init; }
        public int? QualificationId { get; init; }
    }
}
