using Microsoft.AspNetCore.Mvc.Rendering;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class GetQualificationsStep : ISearchStep
    {
        private readonly IProviderSearchService _providerSearchService;

        public GetQualificationsStep(IProviderSearchService providerSearchService)
        {
            _providerSearchService = providerSearchService;
        }

        public async Task Execute(ISearchContext context)
        {
            var qualifications = _providerSearchService.GetQualifications();
            var qualificationSelectListItems = qualifications.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString(), Selected = (q.Id == context.ViewModel.SelectedQualificationId.Value) });
            context.ViewModel.Qualifications = qualificationSelectListItems;
        }
    }
}
