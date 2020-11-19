using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ConfigurationOptions _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IAsyncNotificationClient _notificationClient;

        public EmailService(ConfigurationOptions configuration,
            IAsyncNotificationClient notificationClient,
            ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(notificationClient));
        }

        public async Task<bool> SendEmployerEmail(
            string fullName,
            string organisationName,
            string phoneNumber,
            string email,
            ContactMethod contactMethod)
        {
            //split addresses - send in loop
            //TODO: Handle empty address - add test
            var toAddresses = _configuration.SupportEmailInboxAddress?.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (!toAddresses.Any())
            {
                _logger.LogError("There are no support email addresses defined.");
                return false;
            }

            var tokens = new Dictionary<string, string>
            {
                {"full_name", fullName},
                {"organisation_name", organisationName},
                {"phone_number", phoneNumber},
                {"email_address", email},
                {"contact_method", contactMethod.ToString()}
            };

            foreach (var toAddress in toAddresses)
            {
                await SendEmailAndSaveHistoryAsync(toAddress,
                    _configuration.EmployerContactEmailTemplateId,
                    tokens);
            }

            return true;
        }
        
        private async Task SendEmailAndSaveHistoryAsync(string recipient, string emailTemplateId,
            IDictionary<string, string> personalisationTokens)
        {
            try
            {
                var tokens = personalisationTokens.Select(x => new { key = x.Key, val = (dynamic)x.Value })
                    .ToDictionary(item => item.key, item => item.val);

                var emailResponse = await _notificationClient.SendEmailAsync(recipient, emailTemplateId, tokens);

                _logger.LogInformation($"Email sent - notification id '{emailResponse.id}', " +
                                       $"reference '{emailResponse.reference}, " +
                                       $"content '{emailResponse.content}'");
            }
            catch (Exception ex)
            {
                var message = $"Error sending email template {emailTemplateId} to {recipient}. {ex.Message}";
                _logger.LogError(ex, message);
            }
        }
    }
}
