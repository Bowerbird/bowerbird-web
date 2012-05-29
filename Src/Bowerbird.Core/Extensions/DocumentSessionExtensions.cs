/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using System;

namespace Bowerbird.Core.Repositories
{
    public static class DocumentSessionExtensions
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
