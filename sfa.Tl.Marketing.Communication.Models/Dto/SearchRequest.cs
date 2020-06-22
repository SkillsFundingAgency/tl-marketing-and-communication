namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class SearchRequest
    {
        public string Postcode { get; set; }
        public int NumberOfItems { get; set; }
        public int? QualificationId { get; set; }
    }
}
