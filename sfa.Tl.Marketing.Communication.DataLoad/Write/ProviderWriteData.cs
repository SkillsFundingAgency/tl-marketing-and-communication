using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.DataLoad.Write
{
    public class ProviderWriteData
    {
        public int Id { get; set; }
        public long UkPrn { get; set; }
        public string Name { get; set; }
        public List<LocationWriteData> Locations { get; set; } = new();
    }
}