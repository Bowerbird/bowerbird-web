/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using FluentEmail;

namespace Bowerbird.Core.EventHandlers
{
    public class SendRequestPasswordResetEmailEventHandler : IEventHandler<RequestPasswordResetEvent>
    {
            
        #region Members

        private readonly IEmailService _emailService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public SendRequestPasswordResetEmailEventHandler(
            IEmailService emailService,
            IConfigService configService)
        {
            Check.RequireNotNull(emailService, "emailService");
            Check.RequireNotNull(configService, "configService");

            _emailService = emailService;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(RequestPasswordResetEvent requestPasswordResetEvent)
        {
            Check.RequireNotNull(requestPasswordResetEvent, "requestPasswordResetEvent");

            var message = Email
                .From(_configService.GetEmailAdminAccount(), "Bowerbird")
                .To(requestPasswordResetEvent.User.Email)
                .Subject("Bowerbird password reset request")
                .UsingTemplateFromResource("RequestPasswordResetEmail", new { requestPasswordResetEvent.User.FirstName, ResetUri = string.Format(_configService.GetEmailResetPasswordUri(), requestPasswordResetEvent.User.ResetPasswordKey)})
                .Message;

            _emailService.SendMailMessage(message);
        }

        #endregion

    }
}
