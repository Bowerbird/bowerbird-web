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
    public interface ISightingViewModelBuilder
    {
        object BuildNewObservation(string category = "", string projectId = "");

        object BuildNewRecord(string projectId = "");

        dynamic BuildSighting(string id);

        object BuildGroupSightingList(string groupId, PagingInput pagingInput);

        object BuildUserSightingList(string userId, PagingInput pagingInput);

        object BuildAllUserProjectsSightingList(string userId, PagingInput pagingInput);
    }
}