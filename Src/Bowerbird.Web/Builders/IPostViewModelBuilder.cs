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

namespace Bowerbird.Web.Builders
{
    public interface IPostViewModelBuilder : IBuilder
    {
        object BuildNewPost(string groupId);

        object BuildPost(string postId);

        object BuildUserPostList(string userId, PagingInput pagingInput);

        object BuildGroupPostList(string groupId, PagingInput pagingInput);

        object BuildAllUserGroupsPostList(string userId, PagingInput pagingInput);
    }
}