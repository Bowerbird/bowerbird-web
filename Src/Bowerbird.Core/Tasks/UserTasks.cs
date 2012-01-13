using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Core.Tasks
{
    public class UserTasks : IUserTasks
    {

        #region Members

        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public UserTasks(
            IRepository<User> userRepository)
        {
            Check.RequireNotNull(userRepository, "userRepository");

            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool AreCredentialsValid(string email, string password)
        {
            Check.RequireNotNullOrWhitespace(email, "email");
            Check.RequireNotNullOrWhitespace(password, "password");

            var user = _userRepository.LoadByEmail(email);

            return user != null && user.ValidatePassword(password);
        }

        public bool EmailExists(string email)
        {
            Check.RequireNotNullOrWhitespace(email, "email");

            return _userRepository.LoadByEmail(email) != null;
        }

        public bool ResetPasswordKeyExists(string resetPasswordKey)
        {
            return _userRepository.LoadByResetPasswordKey(resetPasswordKey) != null;
        }

        #endregion      
      
    }
}
