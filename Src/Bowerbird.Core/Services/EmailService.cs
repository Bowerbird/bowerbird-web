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

using System.Net.Mail;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Services
{
    public class EmailService : IEmailService
    {
            
        #region Members

        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public EmailService(
            IConfigService configService)
        {
            Check.RequireNotNull(configService, "configService");

            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendMailMessage(MailMessage mailMessage)
        {
            var smtpClient = new SmtpClient();

            smtpClient.SendAsync(mailMessage, null);
        }

        #endregion
     
    }
}
