using System;
using System.Net.Mail;
using Bowerbird.Core.Config;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using FluentEmail;

namespace Bowerbird.Core.EventHandlers
{
    public class SendWelcomeEmailEventHandler : IEventHandler<DomainModelCreatedEvent<User>>
    {

        #region Members

        private readonly IEmailService _emailService;

        #endregion

        #region Constructors

        public SendWelcomeEmailEventHandler(
            IEmailService emailService)
        {
            Check.RequireNotNull(emailService, "emailService");

            _emailService = emailService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<User> userCreatedEvent)
        {
            Check.RequireNotNull(userCreatedEvent, "userCreatedEvent");

            var message = Email
                .From("fradocaj@museum.vic.gov.au", "Bowerbird")
                .To(userCreatedEvent.DomainModel.Email)
                .Subject("Bowerbird account verification")
                .UsingTemplate("Dear @Model.FirstName,\n\nWelcome to Bowerbird!\n\nTo verify that you email address is valid, please click on the link below, or copy and paste it into your web browser:\n\nhttp://www.bowerbird.org.au/account/verify/3748EE7667GTUUDd889sdjuudhhd77\n\nRegards,\nThe Bowerbird Team\n", userCreatedEvent.DomainModel)
                .Message;

            _emailService.SendMailMessage(message);
        }

        #endregion      

    }
}
