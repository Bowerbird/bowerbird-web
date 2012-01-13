using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.Services;
using FluentEmail;

namespace Bowerbird.Core.EventHandlers
{
    public class SendRequestPasswordResetEmailEventHandler : IEventHandler<RequestPasswordResetEvent>
    {
            
        #region Members

        private readonly IEmailService _emailService;

        #endregion

        #region Constructors

        public SendRequestPasswordResetEmailEventHandler(
            IEmailService emailService)
        {
            Check.RequireNotNull(emailService, "emailService");

            _emailService = emailService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(RequestPasswordResetEvent requestPasswordResetEvent)
        {
            var message = Email
                .From("fradocaj@museum.vic.gov.au", "Bowerbird")
                .To(requestPasswordResetEvent.User.Email)
                .Subject("Bowerbird password reset request")
                .UsingTemplate("Dear @Model.FirstName,\n\nTo reset your email address, click on the link below, or copy and paste it into your web browser:\n\nhttp://www.bowerbird.org.au/account/passwordreset/3748EE7667GTUUDd889sdjuudhhd77\n\nIf you did not request a password reset, please disregard this email.\n\nRegards,\nThe Bowerbird Team\n", requestPasswordResetEvent.User)
                .Message;

            _emailService.SendMailMessage(message);
        }

        #endregion

    }
}
