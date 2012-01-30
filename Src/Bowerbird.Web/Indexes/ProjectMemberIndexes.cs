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
    public class ProjectMember_ByProjectId : AbstractIndexCreationTask<ProjectMember>
    {
        public ProjectMember_ByProjectId()
        {
            Map = projectMembers => projectMembers.Select(x => x.Project.Id);
        }
    }

    public class ProjectMember_ByUserId : AbstractIndexCreationTask<ProjectMember>
    {
        public ProjectMember_ByUserId()
        {
            Map = projects => projects.Select(x => x.User.Id);
        }
    }
}