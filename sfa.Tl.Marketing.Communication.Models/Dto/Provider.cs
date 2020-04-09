using Newtonsoft.Json;
using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Location> Locations { get; set; }
    }
}