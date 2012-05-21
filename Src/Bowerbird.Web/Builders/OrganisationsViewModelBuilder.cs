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
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.Builders
{
    public class OrganisationsViewModelBuilder : IOrganisationsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public OrganisationsViewModelBuilder(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildOrganisation(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.ClientResult>()
                .Where(x => x.GroupId == idInput.Id)
                .FirstOrDefault();

            return MakeOrganisation(organisation);
        }

        public object BuildOrganisationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.ClientResult>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.GroupId)
                .Where(x => x.GroupType == "organisation")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeOrganisation)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Organisation.Id
        /// </summary>
        public object BuildOrganisationUserList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == pagingInput.Id)
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.User.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(x.User.Id))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeOrganisation(All_Groups.ClientResult organisation)
        {
            Check.RequireNotNull(organisation, "organisation");

            return new
            {
                Id = organisation.GroupId,
                organisation.Organisation.Name,
                organisation.Organisation.Description,
                organisation.Organisation.Website,
                Avatar = _avatarFactory.Make(organisation.Organisation),
                Memberships = organisation.Memberships.Count(),
                Teams = organisation.DescendantGroups.Where(x => x.GroupType == "team").Select(x => x.Id),
                Projects = organisation.DescendantGroups.Where(x => x.GroupType == "project").Select(x => x.Id)
            };
        }

        private object MakeUser(string userId)
        {
            return MakeUser(_documentSession.Load<User>(userId));
        }

        private object MakeUser(User user)
        {
            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        #endregion
    }
}