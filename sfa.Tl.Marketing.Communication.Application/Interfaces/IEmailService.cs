using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Enums;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmployerEmail(
            string fullName,
            string organisationName,
            string phoneNumber,
            string email,
            ContactMethod contactMethod);
    }
}
