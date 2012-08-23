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

using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Config;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.EventHandlers
{
    public class SendWelcomeEmail : IEventHandler<DomainModelCreatedEvent<User>>
    {

        #region Members

        private readonly IEmailService _emailService;
        private readonly IConfigSettings _configSettings;

        #endregion

        #region Constructors

        public SendWelcomeEmail(
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

        public void Handle(DomainModelCreatedEvent<User> userCreatedEvent)
        {
            Check.RequireNotNull(userCreatedEvent, "userCreatedEvent");

            var message = Email
                .From("noreply@bowerbird.org.au", "Bowerbird")
                .To(userCreatedEvent.DomainModel.Email)
                .Subject("Bowerbird account verification")
                .UsingTemplate("WelcomeEmail", new { userCreatedEvent.DomainModel.FirstName })
                .Message;

            _emailService.SendMailMessage(message);
        }

        #endregion      

    }
}
