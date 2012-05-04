/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System.Linq;

namespace Bowerbird.Web.Factories
{
    public class MemberViewFactory : IMemberViewFactory
    {
        #region Fields

        protected readonly IUserViewFactory _userViewFactory;
        protected readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberViewFactory(
            IUserViewFactory userViewFactory,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _userViewFactory = userViewFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Member member)
        {
            return new
            {
                member.Id,
                Roles = member.Roles.Select(x => x.Name),
                User = _userViewFactory.Make(_documentSession.Load<User>(member.User.Id)),
                GroupName = member.Group.Name,
                GroupType = GetGroupType(member.Group.Id)
            };
        }

        private static string GetGroupType(string groupId)
        {
            var groupType = groupId.Split('/')[0].ToLower();

            switch (groupType)
            {
                case "organisations":
                    {
                        return "Organisation";
                    }
                case "teams":
                    {
                        return "Team";
                    }
                case "projects":
                    {
                        return "Project";
                    }
                case "approot":
                    {
                        return "Application";
                    }
                case "userprojects":
                    {
                        return "User Project";
                    }
                default:
                    return "Unknown Group Type";
            }
        }

        #endregion
    }
}