/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.Queries
{
    public class OrganisationsQuery : IOrganisationsQuery
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public OrganisationsQuery(
            IUserContext userContext,
            IDocumentSession documentSession,
            IUsersGroupsQuery usersGroupsQuery,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(usersGroupsQuery, "usersGroupsQuery");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _usersGroupsQuery = usersGroupsQuery;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public OrganisationIndex MakeOrganisationIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisation = _documentSession.Load<Organisation>(idInput.Id);

            return new OrganisationIndex()
            {
                Name = organisation.Name,
                Description = organisation.Description,
                Website = organisation.Website,
                Avatar = _avatarFactory.GetAvatar(organisation)
            };
        }

        public OrganisationList MakeOrganisationList(OrganisationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Organisation>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(organisation => new OrganisationView()
                {
                    Id = organisation.Id,
                    Description = organisation.Description,
                    Name = organisation.Name,
                    Website = organisation.Website,
                    Avatar = _avatarFactory.GetAvatar(organisation)
                });

            return new OrganisationList()
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Organisations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public List<OrganisationView> GetGroupsHavingAddTeamPermission()
        {
            var loggedInUserId = _userContext.GetAuthenticatedUserId();

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x =>
                    x.Id.In(_usersGroupsQuery.GetUsersGroupsHavingPermission(loggedInUserId, "createteam")) &&
                    x.GroupType == "organisation"
                )
                .ToList()
                .Select(x => new OrganisationView()
                {
                    Id = x.Id,
                    Description = x.Organisation.Description,
                    Name = x.Organisation.Name,
                    Website = x.Organisation.Website,
                    Avatar = _avatarFactory.GetAvatar(x.Organisation)
                })
                .ToList();
        }

        #endregion
    }
}