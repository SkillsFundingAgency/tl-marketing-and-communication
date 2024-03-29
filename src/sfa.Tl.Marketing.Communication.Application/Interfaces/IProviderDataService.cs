﻿using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface IProviderDataService
{
    IQueryable<ProviderLocation> GetProviderLocations(int? qualificationId = null);

    IEnumerable<Qualification> GetQualifications();
    IEnumerable<Qualification> GetQualifications(int[] qualificationIds);
    Qualification GetQualification(int qualificationId);

    IDictionary<string, string> GetWebsiteUrls();
}