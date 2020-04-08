using System;
using System.Collections.Generic;
using System.Text;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IJsonConvertor
    {
        T DeserializeObject<T>(string json);
    }
}
