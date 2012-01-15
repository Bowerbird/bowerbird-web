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

        #endregion

        #region Constructors

        public UserRequestPasswordResetCommandHandler(
            IRepository<User> userRepository)
        {
            Check.RequireNotNull(userRepository, "userRepository");

            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserRequestPasswordResetCommand userRequestPasswordResetCommand)
        {
            Check.RequireNotNull(userRequestPasswordResetCommand, "userRequestPasswordResetCommand");

            var user = _userRepository.LoadByEmail(userRequestPasswordResetCommand.Email);

            user.RequestPasswordReset();

            _userRepository.Add(user);
        }

        #endregion      
    }
}
