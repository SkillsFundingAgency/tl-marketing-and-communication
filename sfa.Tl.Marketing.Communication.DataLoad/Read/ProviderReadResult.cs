using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.DataLoad.Read

{
    public class ProviderReadResult
    {
        public List<ProviderReadData> Providers { get; set; }
        public string Error { get; set; }
    }
}