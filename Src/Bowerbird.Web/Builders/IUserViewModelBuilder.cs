/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Web.ViewModels;
using System.Collections;

namespace Bowerbird.Web.Builders
{
    public interface IUserViewModelBuilder : IBuilder
    {
        object BuildUser(string userId);

        object BuildUserList(PagingInput pagingInput);

        //object BuildUsersFollowingList(PagingInput pagingInput);

        //object BuildUsersBeingFollowedByList(PagingInput pagingInput);

        object BuildAuthenticatedUser();

        object BuildOnlineUsers();
    }
}