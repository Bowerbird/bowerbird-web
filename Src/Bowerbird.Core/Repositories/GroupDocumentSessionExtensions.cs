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

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Raven.Client;

namespace Bowerbird.Core.Repositories
{
    public static class GroupDocumentSessionExtensions
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static Group LoadGroupById(this IDocumentSession documentSession, string groupId, out string groupType)
        {
            groupType = groupId.GroupType();

            var group = groupId.Split('/')[0].ToLower();

            switch (group)
            {
                case "organisations":
                    {
                        return documentSession.Load<Organisation>(groupId);
                    }
                case "teams":
                    {
                        return documentSession.Load<Team>(groupId);
                    }
                case "projects":
                    {
                        return documentSession.Load<Project>(groupId);
                    }
                case "approot":
                    {
                        return documentSession.Load<AppRoot>(groupId);
                    }
                case "userproject":
                    {
                        return documentSession.Load<UserProject>(groupId);
                    }
                default:
                    return null;
            }
        }

        #endregion      
      
    }
}
