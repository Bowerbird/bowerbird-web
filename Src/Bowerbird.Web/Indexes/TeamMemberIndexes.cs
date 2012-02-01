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
    public class TeamMember_WithTeamIdAndUserId : AbstractIndexCreationTask<TeamMember>
    {
        public TeamMember_WithTeamIdAndUserId()
        {
            Map = teamMembers => from teamMember in teamMembers
                                 select new { TeamId = teamMember.Team.Id, UserId = teamMember.User.Id};
        }
    }
}