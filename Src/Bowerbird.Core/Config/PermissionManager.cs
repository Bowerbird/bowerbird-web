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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.Config
{
    public class PermissionChecker : IPermissionManager
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PermissionChecker(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool HasGroupPermission(string permissionId, string userId, string groupId)
        {
            var user = _documentSession.Load<User>(userId);

            return user.Memberships.Where(x => x.Group.Id == groupId).SelectMany(x => x.Roles).SelectMany(x => x.Permissions).Any(x => x.Id == "permissions/" + permissionId.ToLower());
        }

        public bool HasGroupPermission<T>(string permissionId, string userId, string domainModelId)
            where T : IOwnable
        {
            var user = _documentSession.Load<User>(userId);

            T ownable = _documentSession.Load<T>(domainModelId);

            // 1. Check if user is owner
            if ((((IOwnable)ownable).User.Id == userId))
            {
                return true;
            }

            // 2. Check if user has a valid permission
            return user.Memberships.Where(x => ownable.Groups.Any(y => x.Group.Id == y)).SelectMany(x => x.Roles).SelectMany(x => x.Permissions).Any(x => x.Id == "permissions/" + permissionId.ToLower());
        }

        public bool DoesExist<T>(string id)
        {
            return _documentSession.Load<T>(id) != null;
        }

        #endregion

    }
}
