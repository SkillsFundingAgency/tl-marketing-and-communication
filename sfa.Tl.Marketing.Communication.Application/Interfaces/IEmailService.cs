using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Enums;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmployerContactEmail(
            string fullName,
            string organisationName,
            string phone,
            string email,
            ContactMethod contactMethod);
    }
}
