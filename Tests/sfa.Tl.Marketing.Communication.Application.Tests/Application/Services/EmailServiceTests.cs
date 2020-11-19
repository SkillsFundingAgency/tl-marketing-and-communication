using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class EmailServiceTests
    {
        private const string EmailTemplateId = "60fb5cff-dc16-4593-ab42-e2c8716f14f8";
        private const string MainSupportEmailInboxAddress = "support@marcomms.com";
        private const string SecondarySupportEmailInboxAddress = "moresupport@marcomms.com";

        private const string TestFullName = "Name";
        private const string TestOrganisation = "Organisation";
        private const string TestPhoneNumber = "012-3456-78";
        private const string TestEmail = "test@test.com";

        [Fact]
        public async Task EmailService_Sends_Email()
        {
            var notificationClient = Substitute.For<IAsyncNotificationClient>();

            var emailService = BuildEmailService(notificationClient: notificationClient);

            var result = await SendEmployerEmail(emailService);

            result.Should().Be(true);

            await notificationClient
                .Received(1)
                .SendEmailAsync(Arg.Is<string>(emailAddress =>
                        emailAddress == MainSupportEmailInboxAddress),
                    Arg.Is<string>(templateId =>
                        templateId == EmailTemplateId),
                    Arg.Any<Dictionary<string, dynamic>>());
            //Arg.Is<Dictionary<string, dynamic>>(dict =>
            //    dict.First().Key == "contactname"));
        }

        [Fact]
        public async Task EmailService_Sends_Email_With_Expected_Tokens()
        {
            var notificationClient = Substitute.For<IAsyncNotificationClient>();

            var emailService = BuildEmailService(notificationClient: notificationClient);

            var result = await emailService.SendEmployerEmail(
                TestFullName,
                TestOrganisation,
                TestPhoneNumber,
                TestEmail,
                ContactMethod.Phone);

            result.Should().Be(true);

            await notificationClient
                .Received(1)
                .SendEmailAsync(Arg.Is<string>(emailAddress =>
                        emailAddress == MainSupportEmailInboxAddress),
                    Arg.Is<string>(templateId =>
                        templateId == EmailTemplateId),
            Arg.Is<Dictionary<string, dynamic>>(tokens =>
                    tokens.ContainsKey("full_name") && //tokens["full_name"].ToString() == TestFullName &&
                    tokens.ContainsKey("organisation_name") && //tokens["organisation_name"] == TestFullName &&
                    tokens.ContainsKey("phone_number") && //tokens["phone_number"] == TestFullName &&
                    tokens.ContainsKey("email_address") && //tokens["email_address"] == TestFullName &&
                    tokens.ContainsKey("contact_method") //&& tokens["contact_method"] == TestFullName
                    ));
        }

        [Fact]
        public async Task EmailService_Sends_No_Emails_When_Inbox_Address_Is_Empty()
        {
            var notificationClient = Substitute.For<IAsyncNotificationClient>();
            var emailService = BuildEmailService(supportInbox: "",
                notificationClient: notificationClient);

            var result = await SendEmployerEmail(emailService);

            //result.Should().BeFalse();

            await notificationClient
                .DidNotReceive()
                .SendEmailAsync(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<Dictionary<string, dynamic>>());
        }

        [Fact]
        public async Task EmailService_Sends_Emails_To_Two_Recipients()
        {
            var notificationClient = Substitute.For<IAsyncNotificationClient>();
            var emailAddressList = $"{MainSupportEmailInboxAddress};{SecondarySupportEmailInboxAddress}";

            var emailService = BuildEmailService(supportInbox: emailAddressList,
                notificationClient: notificationClient);

            var result = await SendEmployerEmail(emailService);

            await notificationClient
                .Received(2)
                .SendEmailAsync(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<Dictionary<string, dynamic>>());

            await notificationClient
                .Received(1)
                .SendEmailAsync(Arg.Is<string>(emailAddress =>
                        emailAddress == MainSupportEmailInboxAddress),
                    Arg.Any<string>(),
                    Arg.Any<Dictionary<string, dynamic>>());

            await notificationClient
                .Received(1)
                .SendEmailAsync(Arg.Is<string>(emailAddress =>
                        emailAddress == MainSupportEmailInboxAddress),
                    Arg.Any<string>(),
                    Arg.Any<Dictionary<string, dynamic>>());

        }

        private async Task<bool> SendEmployerEmail(IEmailService emailService)
        {
            return await emailService.SendEmployerEmail(
                TestFullName,
                TestOrganisation,
                TestPhoneNumber,
                TestEmail,
                ContactMethod.Phone);
        }

        private IEmailService BuildEmailService(
            string emailTemplateId = null,
            string supportInbox = null,
            IAsyncNotificationClient notificationClient = null,
            ILogger<EmailService> logger = null)
        {
            notificationClient ??= Substitute.For<IAsyncNotificationClient>();
            logger ??= Substitute.For<ILogger<EmailService>>();

            var configuration = new ConfigurationOptions
            {
                EmployerContactEmailTemplateId = emailTemplateId ?? EmailTemplateId,
                SupportEmailInboxAddress = supportInbox ?? MainSupportEmailInboxAddress
            };

            return new EmailService(configuration, notificationClient, logger);
        }
    }
}
