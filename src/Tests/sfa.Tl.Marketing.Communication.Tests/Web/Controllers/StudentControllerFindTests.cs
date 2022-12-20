using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers;

public class StudentControllerFindTests
{
    private const string TestPostcode = "CV1 2WT";

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

        var result = await controller.Find();

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.Model.Should().BeOfType(typeof(FindViewModel));
        var searchResult = viewResult.Model as FindViewModel;
        searchResult.Should().NotBeNull();
        searchResult!.SearchTerm.Should().BeNull();

        await providerSearchEngine
                   .Received(1)
                   .Search(Arg.Is<FindViewModel>(vm => vm.SearchTerm == null));
    }

    [Fact]
    public async Task Student_Controller_Find_Get_Returns_Expected_Value_When_Session_Data_Exists()
    {
        var providerSearchEngine = Substitute.For<IProviderSearchEngine>();
        providerSearchEngine.Search(
                Arg.Any<FindViewModel>())
            .Returns(args => (FindViewModel)args[0]);

        var viewModel = new FindViewModel { SearchTerm = TestPostcode };
        var serializedValue = JsonSerializer.Serialize(viewModel);

        var session = Substitute.For<ISession>();
        session.TryGetValue(SessionKeys.FindViewModelKey, out Arg.Any<byte[]>())
            .Returns(x =>
            {
                x[1] = Encoding.UTF8.GetBytes(serializedValue);
                return true;
            });

        var controller = new StudentControllerBuilder().Build(
            providerSearchEngine: providerSearchEngine,
            session: session);

        var result = await controller.Find();

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.Model.Should().BeOfType(typeof(FindViewModel));
        var searchResult = viewResult.Model as FindViewModel;
        searchResult.Should().NotBeNull();
        searchResult!.SearchTerm.Should().Be(TestPostcode);

        await providerSearchEngine
            .Received(1)
            .Search(Arg.Is<FindViewModel>(vm => vm.SearchTerm == TestPostcode));
    }

    [Fact]
    public void Student_Controller_Find_Post_Returns_Expected_Value()
    {
        var session = Substitute.For<ISession>();

        var controller = new StudentControllerBuilder().Build(
            session: session);

        var viewModel = new FindViewModel
        {
            SearchTerm = TestPostcode
        };

        var expectedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(viewModel));

        var result = controller.Find(viewModel);

        var redirectResult = result as RedirectToRouteResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Permanent.Should().BeFalse();
        redirectResult.RouteName.Should().Be("Find");
        redirectResult.PreserveMethod.Should().BeFalse();

        session
            .Received(1)
            .Set(SessionKeys.FindViewModelKey, Arg.Any<byte[]>());

        session
            .Received(1)
            .Set(SessionKeys.FindViewModelKey, 
                Arg.Is<byte[]>(b => b.SequenceEqual(expectedBytes)));
    }
}