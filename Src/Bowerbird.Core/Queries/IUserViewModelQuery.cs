/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.ViewModels;
using System.Collections;

namespace Bowerbird.Core.Queries
{
    public interface IUserViewModelQuery : IQuery
    {
        object BuildUser(string userId, bool fullDetails = false);

        object BuildUpdateUser(string userId);

        object BuildAuthenticatedUser(string userId);

        object BuildGroupUserList(string groupId, UsersQueryInput usersQueryInput);

        object BuildGroupUserList(string groupId, string role);

        object BuildOnlineUserList(string authenticatedUserId);

        object BuildUserList(UsersQueryInput usersQueryInput);
    }
}