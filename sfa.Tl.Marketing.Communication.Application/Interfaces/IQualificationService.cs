using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IQualificationService
    {
        IEnumerable<Qualification> GetQualifications();
        IEnumerable<Qualification> GetQualifications(int[] qualificationIds);
    }
}
