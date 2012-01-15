using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using FluentEmail;

namespace Bowerbird.Core.Services
{
    public class EmailService : IEmailService
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendMailMessage(MailMessage mailMessage)
        {
            var emailServerName = ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).ServerName;

            var smtpClient = new SmtpClient(emailServerName);

            smtpClient.SendAsync(mailMessage, null);
        }

        #endregion
     
    }
}
