﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Net.Mail;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.Services
{
    public class EmailService : IEmailService
    {
            
        #region Members

        private readonly IConfigService _configService;
        private readonly ISystemStateManager _systemStateManager;

        #endregion

        #region Constructors

        public EmailService(
            IConfigService configService,
            ISystemStateManager systemStateManager)
        {
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(systemStateManager, "systemStateManager");

            _configService = configService;
            _systemStateManager = systemStateManager;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendMailMessage(MailMessage mailMessage)
        {
            var smtpClient = new SmtpClient();

            if (!_systemStateManager.SendEmails)
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            }

            smtpClient.SendAsync(mailMessage, null);
        }

        #endregion
     
    }
}
