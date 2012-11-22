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

        object BuildUpdateUser(string userId);

        object BuildAuthenticatedUser(string userId);

        object BuildGroupUserList(string groupId, PagingInput pagingInput);

        object BuildOnlineUserList();
    }
}