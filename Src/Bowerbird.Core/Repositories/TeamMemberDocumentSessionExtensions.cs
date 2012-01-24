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

    public static class TeamMemberDocumentSessionExtensions
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static TeamMember LoadTeamMember(this IDocumentSession documentSession, string teamId, string userId)
        {
            //string actualProjectId = "projects/" + projectId;
            //string actualUserId = "users/" + userId;

            return documentSession
                .Query<TeamMember>()
                .Where(x => x.Team.Id == teamId && x.User.Id == userId)
                .FirstOrDefault();
        }

        #endregion

    }
}