using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers;

public class StudentControllerFindTests
{
    [Fact]
    public void Student_Controller_Constructor_Guards_Against_NullParameters()
    {
        typeof(StudentController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Student_Controller_Find_Get_Returns_Expected_Value()
    {
        var providerSearchEngine = Substitute.For<IProviderSearchEngine>();
        providerSearchEngine.Search(
                Arg.Any<FindViewModel>())
            .Returns(args => (FindViewModel)args[0]);

        var controller = new StudentControllerBuilder().BuildStudentController(providerSearchEngine: providerSearchEngine);

        var viewModel = new FindViewModel();
        var result = await controller.Find(viewModel);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult?.Model.Should().BeOfType(typeof(FindViewModel));

        await providerSearchEngine
            .Received(1)
            //.Search(Arg.Is<FindViewModel>(x => x != null));
            .Search(viewModel);
    }
}