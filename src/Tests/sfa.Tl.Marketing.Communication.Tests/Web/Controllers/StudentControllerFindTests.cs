using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

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

        var session = Substitute.For<ISession>();
        session.TryGetValue(Arg.Any<string>(), out _)
            .Returns(false);

        var controller = new StudentControllerBuilder().Build(
            providerSearchEngine: providerSearchEngine,
            session: session);

        var viewModel = new FindViewModel();
        var result = await controller.Find(viewModel);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.Model.Should().BeOfType(typeof(FindViewModel));
        viewResult.Model.Should().Be(viewModel);
        
        var searchResult = viewResult.Model as FindViewModel;
        searchResult.Should().NotBeNull();
        searchResult!.SearchTerm.Should().BeNull();

        await providerSearchEngine
                   .Received(1)
                   .Search(Arg.Is<FindViewModel>(vm => vm.SearchTerm == null));
    }
}