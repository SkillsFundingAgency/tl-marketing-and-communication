using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class ProviderLocationViewModel
    {
        public string ProviderName { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DistanceInMiles { get; set; }
        public string DistanceString
        {
            get
            {
                return DistanceInMiles == 1 ? "mile" : "miles";
            }
        }
        public IEnumerable<QualificationViewModel> Qualification2020 { get; set; }
        public IEnumerable<QualificationViewModel> Qualification2021 { get; set; }
        public string Website { get; set; }
        public string RedirectUrl
        {
            get
            {
                return $"/students/redirect?url={Website}";
            }
        }

        public string RedirectUrlLabel        
        {
            get
            {
                return $"Visit {VenueName}'s website";
            }
        }

        public string VenueName
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? ProviderName : Name;
            }
        }

        public string AddressLabel
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? $"Part of {ProviderName} \r\n {Town} | {Postcode}" : $"{Town} | {Postcode}";
            }
        }
    }
}
