/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client.Indexes;

namespace Bowerbird.Web.Indexes
{
    public class TeamMember_ByTeamId : AbstractIndexCreationTask<TeamMember>
    {
        public TeamMember_ByTeamId()
        {
            Map = teamMembers => teamMembers.Select(x => x.Team.Id);
        }
    }

    public class TeamMember_ByUserId : AbstractIndexCreationTask<TeamMember>
    {
        public TeamMember_ByUserId()
        {
            Map = teamMembers => teamMembers.Select(x => x.User.Id);
        }
    }
}
