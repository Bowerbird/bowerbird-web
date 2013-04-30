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
using Bowerbird.Core.Config;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;

namespace Bowerbird.Core.EventHandlers
{
    public class SendRequestPasswordResetEmail : IEventHandler<RequestPasswordUpdate>
    {
            
        #region Members

        private readonly IEmailService _emailService;
        private readonly IConfigSettings _configSettings;

        #endregion

        #region Constructors

        public SendRequestPasswordResetEmail(
            IEmailService emailService,
            IConfigSettings configService)
        {
            Check.RequireNotNull(emailService, "emailService");
            Check.RequireNotNull(configService, "configService");

            _emailService = emailService;
            _configSettings = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(RequestPasswordUpdate requestPasswordUpdate)
        {
            Check.RequireNotNull(requestPasswordUpdate, "requestPasswordUpdate");

            if (requestPasswordUpdate.SendEmail)
            {
                var message = Email
                    .From(_configSettings.GetEmailAdminAccount(), "BowerBird")
                    .To(requestPasswordUpdate.User.Email)
                    .Subject("Reset your BowerBird password")
                    .UsingTemplate("RequestPasswordResetEmail", new { requestPasswordUpdate.User.Name, ResetUri = string.Format(_configSettings.GetEnvironmentRootUri() + _configSettings.GetEmailResetPasswordRelativeUri(), requestPasswordUpdate.User.ResetPasswordKey) })
                    .Message;

                _emailService.SendMailMessage(message);
            }
        }

        #endregion

    }
}
