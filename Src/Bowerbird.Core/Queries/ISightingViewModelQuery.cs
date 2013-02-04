/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Queries
{
    public interface ISightingViewModelQuery
    {
        object BuildCreateObservation(string category = "", string projectId = "");

        object BuildCreateRecord(string projectId = "");

        object BuildSighting(string id);

        object BuildSightingList(SightingsQueryInput sightingsQueryInput);

        object BuildGroupSightingList(string groupId, SightingsQueryInput sightingsQueryInput);

        object BuildHomeSightingList(string userId, SightingsQueryInput sightingsQueryInput);
    }
}