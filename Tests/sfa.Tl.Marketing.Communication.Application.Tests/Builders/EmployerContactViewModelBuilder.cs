using sfa.Tl.Marketing.Communication.Enums;
using sfa.Tl.Marketing.Communication.Models;
// ReSharper disable UnusedMember.Global

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class EmployerContactViewModelBuilder
    {
        private readonly EmployerContactViewModel _viewModel;

        public EmployerContactViewModelBuilder()
        {
            _viewModel = new EmployerContactViewModel();
        }

        public EmployerContactViewModel Build() => _viewModel;

        public EmployerContactViewModelBuilder WithDefaultValues()
        {
            _viewModel.FullName = "";
            _viewModel.OrganisationName = "Test Co";
            _viewModel.Email = "employer@test.com";
            _viewModel.PhoneNumber = "0345 555 5555";
            _viewModel.ContactMethod = ContactMethod.Email;
            return this;
        }

        public EmployerContactViewModelBuilder WithFullName(string fullName)
        {
            _viewModel.FullName = fullName;
            return this;
        }
        
        public EmployerContactViewModelBuilder With(string organisationName)
        {
            _viewModel.OrganisationName = organisationName;
            return this;
        }

        public EmployerContactViewModelBuilder WithEmail(string email)
        {
            _viewModel.Email = email;
            return this;
        }

        public EmployerContactViewModelBuilder WithPhoneNumber(string phoneNumber)
        {
            _viewModel.PhoneNumber = phoneNumber;
            return this;
        }

        public EmployerContactViewModelBuilder With(ContactMethod contactMethod)
        {
            _viewModel.ContactMethod = contactMethod;
            return this;
        }
    }
}
