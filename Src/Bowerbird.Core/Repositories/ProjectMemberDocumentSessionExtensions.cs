/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Raven.Client;

namespace Bowerbird.Core.Repositories
{

    public static class ProjectMemberDocumentSessionExtensions
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static ProjectMember LoadProjectMember(this IDocumentSession documentSession, string projectId, string userId)
        {
            string actualProjectId = "projects/" + projectId;
            string actualUserId = "users/" + userId;

            return documentSession
                .Query<ProjectMember>()
                .Where(x => x.Project.Id == actualProjectId && x.User.Id == actualUserId)
                .FirstOrDefault();
        }

        #endregion

    }
}