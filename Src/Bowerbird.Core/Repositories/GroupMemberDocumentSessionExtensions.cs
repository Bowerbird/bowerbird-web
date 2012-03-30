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
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{

    public static class MemberDocumentSessionExtensions
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static Member LoadMember(this IDocumentSession documentSession, string groupId, string userId)
        {
            return documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == groupId && x.User.Id == userId)
                .FirstOrDefault();
        }

        #endregion

    }
}