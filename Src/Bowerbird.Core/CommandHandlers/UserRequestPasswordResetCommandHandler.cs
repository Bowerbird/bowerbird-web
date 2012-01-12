using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserRequestPasswordResetCommandHandler : ICommandHandler<UserRequestPasswordResetCommand>
    {
        #region Members

        private readonly IRepository<User> _userRepository;
        private readonly IEmailService _emailService;

        #endregion

        #region Constructors

        public UserRequestPasswordResetCommandHandler(
            IRepository<User> userRepository,
            IEmailService emailService)
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(emailService, "emailService");

            _userRepository = userRepository;
            _emailService = emailService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserRequestPasswordResetCommand userRequestPasswordResetCommand)
        {
            Check.RequireNotNull(userRequestPasswordResetCommand, "userRequestPasswordResetCommand");

            var user = _userRepository.LoadByEmail(userRequestPasswordResetCommand.Email);

            user.UpdateResetPasswordKey(userRequestPasswordResetCommand.ResetPasswordKey);

            _userRepository.Add(user);

            _emailService.SendEmail(user);
        }

        #endregion      
    }
}
