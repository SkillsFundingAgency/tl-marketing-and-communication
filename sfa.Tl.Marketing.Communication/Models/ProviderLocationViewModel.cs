﻿using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
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
        public IEnumerable<Qualification> Qualification2020 { get; set; }
        public IEnumerable<Qualification> Qualification2021 { get; set; }
        public string Website { get; set; }
    }
}