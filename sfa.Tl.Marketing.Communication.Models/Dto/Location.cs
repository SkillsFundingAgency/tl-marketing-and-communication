﻿using Newtonsoft.Json;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Location
    {
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int[] Qualification2020 { get; set; }
        public int[] Qualification2021 { get; set; }
        public string Website { get; set; }
    }
}