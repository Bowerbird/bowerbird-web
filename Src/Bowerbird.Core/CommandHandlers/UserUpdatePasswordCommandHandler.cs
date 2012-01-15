using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdatePasswordCommandHandler : ICommandHandler<UserUpdatePasswordCommand>
    {
        #region Members

        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public UserUpdatePasswordCommandHandler(
            IRepository<User> userRepository)
        {
            Check.RequireNotNull(userRepository, "userRepository");

            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdatePasswordCommand userUpdatePasswordCommand)
        {
            Check.RequireNotNull(userUpdatePasswordCommand, "userUpdatePasswordCommand");

            User user = null;

            if (!string.IsNullOrWhiteSpace(userUpdatePasswordCommand.ResetPasswordKey))
            {
                user = _userRepository.LoadByResetPasswordKey(userUpdatePasswordCommand.ResetPasswordKey);
            }

            if(!string.IsNullOrWhiteSpace(userUpdatePasswordCommand.UserId))
            {
                user = _userRepository.Load(userUpdatePasswordCommand.UserId);
            }

            user.UpdatePassword(userUpdatePasswordCommand.Password);

            _userRepository.Add(user);
        }

        #endregion      
    }
}
