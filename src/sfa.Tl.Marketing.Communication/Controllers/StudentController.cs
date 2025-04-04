﻿using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Controllers;

public class StudentController : Controller
{
    private readonly IProviderDataService _providerDataService;
    private readonly IProviderSearchEngine _providerSearchEngine;

    public StudentController(
        IProviderDataService providerDataService,
        IProviderSearchEngine providerSearchEngine)
    {
        _providerSearchEngine = providerSearchEngine ?? throw new ArgumentNullException(nameof(providerSearchEngine));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
    }

    [Route("/students", Name = "Index")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/students/about", Name = "About")]
    public IActionResult About()
    {
        return View();
    }

    [Route("/students/parents", Name = "Parents")]
    public IActionResult Parents()
    {
        return View();
    }

    [Route("/students/subjects", Name = "Subjects")]
    public IActionResult Subjects()
    {
        return View();
    }


    [Route("/students/student-stories", Name = "StudentStories")]
    public IActionResult StudentStories()
    {
        return View();
    }

    [Route("/students/transcripts/home", Name = "StudentHomeTranscript")]
    public IActionResult StudentHomeTranscript()
    {
        return View("Transcripts/StudentHomeTranscript");
    }

    [Route("/students/transcripts/ambassadors1", Name = "StudentAmbassadorTranscript1")]
    public IActionResult StudentAmbassadorTranscript1()
    {
        return View("Transcripts/StudentAmbassadorTranscript1");
    }

    [Route("/students/transcripts/ambassadors2", Name = "StudentAmbassadorTranscript2")]
    public IActionResult StudentAmbassadorTranscript2()
    {
        return View("Transcripts/StudentAmbassadorTranscript2");
    }


    [Route("/students/transcripts/ambassadors3", Name = "StudentAmbassadorTranscript3")]
    public IActionResult StudentAmbassadorTranscript3()
    {
        return View("Transcripts/StudentAmbassadorTranscript3");
    }

    [Route("/students/transcripts/ambassadors4", Name = "StudentAmbassadorTranscript4")]
    public IActionResult StudentAmbassadorTranscript4()
    {
        return View("Transcripts/StudentAmbassadorTranscript4");
    }

    [Route("/students/transcripts/ambassadors5", Name = "StudentAmbassadorTranscript5")]
    public IActionResult StudentAmbassadorTranscript5()
    {
        return View("Transcripts/StudentAmbassadorTranscript5");
    }

    [Route("/students/transcripts/ambassadors6", Name = "StudentAmbassadorTranscript6")]
    public IActionResult StudentAmbassadorTranscript6()
    {
        return View("Transcripts/StudentAmbassadorTranscript6");
    }

    [Route("/students/transcripts/ambassadors7", Name = "StudentAmbassadorTranscript7")]
    public IActionResult StudentAmbassadorTranscript7()
    {
        return View("Transcripts/StudentAmbassadorTranscript7");
    }

    [Route("/students/find", Name = "Find")]
    public async Task<IActionResult> Find(FindViewModel viewModel)
    {
        var searchResults = await _providerSearchEngine.Search(viewModel);

        return View(searchResults);
    }

    [Route("/students/subjects/accounting", Name = "Accounting")]
    public IActionResult Accounting()
    {
        return View("Subjects/Accounting");
    }

    [Route("/students/subjects/building-services-engineering", Name = "BuildingServicesEngineering")]
    public IActionResult BuildingServicesEngineering()
    {
        return View("Subjects/BuildingServicesEngineering");
    }

    [Route("/students/subjects/design-development-engineering", Name = "DesignDevelopmentEngineering")]
    public IActionResult DesignDevelopmentEngineering()
    {
        return View("Subjects/DesignDevelopmentEngineering");
    }

    [Route("/students/subjects/design-surveying-planning", Name = "DesignSurveyingPlanning")]
    public IActionResult DesignSurveyingPlanning()
    {
        return View("Subjects/DesignSurveyingPlanning");
    }

    [Route("/students/subjects/digital-business-services", Name = "DigitalBusinessServices")]
    public IActionResult DigitalBusinessServices()
    {
        return View("Subjects/DigitalBusinessServices");
    }

    [Route("/students/subjects/digital-production-design-development", Name = "DigitalProductionDesignDevelopment")]
    public IActionResult DigitalProductionDesignDevelopment()
    {
        return View("Subjects/DigitalProductionDesignDevelopment");
    }

    [Route("/students/subjects/digital-support-services", Name = "DigitalSupportServices")]
    public IActionResult DigitalSupportServices()
    {
        return View("Subjects/DigitalSupportServices");
    }

    [Route("/students/subjects/education", Name = "Education")]
    public IActionResult Education()
    {
        return View("Subjects/Education");
    }

    [Route("/students/subjects/engineering-manufacturing-processing-control", Name = "EngineeringManufacturingProcessingControl")]
    public IActionResult EngineeringManufacturingProcessingControl()
    {
        return View("Subjects/EngineeringManufacturingProcessingControl");
    }

    [Route("/students/subjects/finance", Name = "Finance")]
    public IActionResult Finance()
    {
        return View("Subjects/Finance");
    }

    [Route("/students/subjects/health", Name = "Health")]
    public IActionResult Health()
    {
        return View("Subjects/Health");
    }

    [Route("/students/subjects/healthcare-science", Name = "HealthcareScience")]
    public IActionResult HealthcareScience()
    {
        return View("Subjects/HealthcareScience");
    }

    [Route("/students/subjects/maintenance-installation-repair", Name = "MaintenanceInstallationRepair")]
    public IActionResult MaintenanceInstallationRepair()
    {
        return View("Subjects/MaintenanceInstallationRepair");
    }

    [Route("/students/subjects/management-administration", Name = "ManagementAdministration")]
    public IActionResult ManagementAdministration()
    {
        return View("Subjects/ManagementAdministration");
    }

    [Route("/students/subjects/science", Name = "Science")]
    public IActionResult Science()
    {
        return View("Subjects/Science");
    }

    [Route("/students/subjects/agriculture-land-management-production", Name = "AgricultureLandManagementProduction")]
    public IActionResult AgricultureLandManagementProduction()
    {
        return View("Subjects/AgricultureLandManagementProduction");
    }

    [Route("/students/subjects/animal-care-management", Name = "AnimalCare")]
    public IActionResult AnimalCare()
    {
        return View("Subjects/AnimalCare");
    }

    [Route("/students/subjects/craft-design", Name = "CraftDesign")]
    public IActionResult CraftDesign()
    {
        return View("Subjects/CraftDesign");
    }

    [Route("/students/subjects/media-broadcast-production", Name = "MediaBroadcastProduction")]
    public IActionResult MediaBroadcastProduction()
    {
        return View("Subjects/MediaBroadcastProduction");
    }

    [Route("/students/subjects/legal-services", Name = "LegalServices")]
    public IActionResult LegalServices()
    {
        return View("Subjects/LegalServices");
    }

    [Route("/students/subjects/marketing", Name = "Marketing")]
    public IActionResult Marketing()
    {
        return View("Subjects/Marketing");
    }

    [Route("/students/redirect", Name = "Redirect")]
    public IActionResult Redirect(RedirectViewModel viewModel)
    {
        var allowedUrls = _providerDataService
            .GetWebsiteUrls();

        //Need to decode the url for comparison to the allow list,
        //as it has been encoded before being added to web pages
        var decodedUrl = WebUtility.UrlDecode(viewModel.Url);
        var targetUrl =
            decodedUrl is not null
            && (allowedUrls.ContainsKey(decodedUrl)
                || Url.IsLocalUrl(decodedUrl))
                ? viewModel.Url
                : "/students";

        return new RedirectResult(targetUrl, false);
    }

    [Route("/student")]
    public IActionResult IndexRedirect()
    {
        return RedirectToAction(nameof(Index));
    }

    [Route("/about", Name = "AboutOld")]
    public IActionResult AboutRedirect()
    {
        return RedirectToAction(nameof(About));
    }

    [Route("/why", Name = "WhyOld")]
    public IActionResult WhyRedirect()
    {
        return RedirectToAction(nameof(About));
    }


    [Route("/students/why", Name = "WhyNew")]
    public IActionResult WhyNewRedirect()
    {
        return RedirectToAction(nameof(About));
    }

    [Route("/subjects", Name = "SubjectsOld")]
    public IActionResult SubjectsRedirect()
    {
        return RedirectToAction(nameof(Subjects));
    }

    [Route("/find", Name = "FindOld")]
    public IActionResult FindRedirect()
    {
        return RedirectToAction(nameof(Find));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}