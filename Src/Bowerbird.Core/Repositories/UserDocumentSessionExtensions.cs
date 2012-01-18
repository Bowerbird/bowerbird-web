using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.Repositories
{
    public static class UserDocumentSessionExtensions
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static User LoadUserByEmail(this IDocumentSession documentSession, string email)
        {
            return documentSession
                .Query<User>()
                .Where(x => x.Email == email)
                .FirstOrDefault();
        }

        public static User LoadUserByResetPasswordKey(this IDocumentSession documentSession, string resetPasswordKey)
        {
            return documentSession
                .Query<User>()
                .Where(x => x.ResetPasswordKey == resetPasswordKey)
                .FirstOrDefault();
        }

        #endregion      
      
    }
}
