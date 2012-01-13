using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public static class UserRepositoryExtensions
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static User LoadByEmail(this IRepository<User> repository, string email)
        {
            return repository
                .Session
                .Query<User>()
                .Where(x => x.Email == email)
                .FirstOrDefault();
        }

        public static User LoadByResetPasswordKey(this IRepository<User> repository, string resetPasswordKey)
        {
            return repository
                .Session
                .Query<User>()
                .Where(x => x.ResetPasswordKey == resetPasswordKey)
                .FirstOrDefault();
        }

        #endregion      
      
    }
}
