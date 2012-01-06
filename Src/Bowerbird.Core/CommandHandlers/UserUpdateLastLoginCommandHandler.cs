using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateLastLoginCommandHandler : ICommandHandler<UserUpdateLastLoginCommand>
    {
        #region Members

        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public UserUpdateLastLoginCommandHandler(
            IRepository<User> userRepository)
        {
            Check.RequireNotNull(userRepository, "userRepository");

            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdateLastLoginCommand userUpdateLastLoginCommand)
        {
            Check.RequireNotNull(userUpdateLastLoginCommand, "userUpdateLastLoginCommand");

            var user = _userRepository.Load("users/" + userUpdateLastLoginCommand.UserId);

            user.UpdateLastLoggedIn();

            _userRepository.Add(user);
        }

        #endregion      
    }
}