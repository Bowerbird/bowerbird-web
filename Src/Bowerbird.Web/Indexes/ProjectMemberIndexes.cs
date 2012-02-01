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
    public class ProjectMember_WithProjectIdAndUserId : AbstractIndexCreationTask<ProjectMember>
    {
        public ProjectMember_WithProjectIdAndUserId()
        {
            Map = projectMembers => from projectMember in projectMembers
                                    select new { UserId = projectMember.User.Id, ProjectId = projectMember.Project.Id };
        }
    }
}