/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Paging;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Queries
{
    public interface IActivityViewModelQuery
    {
        /// <summary>
        /// Builds user's home page activity list
        /// </summary>
        object BuildHomeActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput);

        /// <summary>
        /// Builds a user's public activity list
        /// </summary>
        object BuildUserActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput);

        /// <summary>
        /// Builds a group's actvity list
        /// </summary>
        object BuildGroupActivityList(string groupId, ActivityInput activityInput, PagingInput pagingInput);

        /// <summary>
        /// Builds a user's notification actvity list
        /// </summary>
        object BuildNotificationActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput);
    }
}