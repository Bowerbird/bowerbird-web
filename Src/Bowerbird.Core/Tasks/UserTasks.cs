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
            var user = _userRepository.LoadByEmail(email);

            return user != null && user.ValidatePassword(password);
        }

        public string GetEmailByResetPasswordKey(string resetPasswordKey)
        {
            Check.RequireNotNullOrWhitespace(resetPasswordKey, "resetPasswordKey");

            var user = _userRepository.LoadByResetPasswordKey(resetPasswordKey);

            return user.Email;
        }

        public string GetUserIdByResetPasswordKey(string resetPasswordKey)
        {
            Check.RequireNotNullOrWhitespace(resetPasswordKey, "resetPasswordKey");

            var user = _userRepository.LoadByResetPasswordKey(resetPasswordKey);

            return user.Id;
        }

        public string GetUserIdByEmail(string email)
        {
            Check.RequireNotNullOrWhitespace(email, "email");

            var user = _userRepository.LoadByEmail(email);

            return user.Id;
        }
        

        #endregion      
      
    }
}
