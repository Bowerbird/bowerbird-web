using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using FluentEmail;

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
