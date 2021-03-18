using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmployerContactEmail(
            string fullName,
            string organisationName,
            string phone,
            string email);
    }
}
