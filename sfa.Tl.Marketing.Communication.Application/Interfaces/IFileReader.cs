using System;
using System.Collections.Generic;
using System.Text;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IFileReader
    {
        string ReadAllText(string filePath);
    }
}
