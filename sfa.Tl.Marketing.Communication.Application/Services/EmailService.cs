using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notify.Interfaces;
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

        public async Task<bool> SendEmployerContactEmail(
            string fullName,
            string organisationName,
            string phone,
            string email)
        {
            var toAddresses = _configuration.SupportEmailInboxAddress?.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (toAddresses == null || !toAddresses.Any())
            {
                _logger.LogError("There are no support email addresses defined.");
                return false;
            }
            
            var tokens = new Dictionary<string, dynamic>
            {
                { "full_name", fullName },
                { "organisation_name", organisationName },
                { "organisation_phone_number", phone },
                { "organisation_email_address", email }
            };

            var allEmailsSent = true;
            foreach (var toAddress in toAddresses)
            {
                allEmailsSent &= await SendEmail(toAddress,
                    _configuration.EmployerContactEmailTemplateId,
                    tokens);
            }

            return allEmailsSent;
        }
        
        private async Task<bool> SendEmail(string recipient, string emailTemplateId,
            Dictionary<string, dynamic> personalisationTokens)
        {
            var emailSent = false;

            try
            {
                var emailResponse = await _notificationClient.SendEmailAsync(recipient, emailTemplateId, personalisationTokens);

                _logger.LogInformation($"Email sent - notification id '{emailResponse.id}', " +
                                       $"reference '{emailResponse.reference}, " +
                                       $"content '{emailResponse.content}'");
                emailSent = true;
            }
            catch (Exception ex)
            {
                var message = $"Error sending email template {emailTemplateId} to {recipient}. {ex.Message}";
                _logger.LogError(ex, message);
            }

            return emailSent;
        }
    }
}
