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
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class OrganisationsViewModelBuilder : IOrganisationsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public OrganisationsViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildOrganisation(string organisationId)
        {
            Check.RequireNotNullOrWhitespace(organisationId, "organisationId");

            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == organisationId)
                .FirstOrDefault();

            return MakeOrganisation(organisation);
        }

        public object BuildOrganisation()
        {
            return new
            {
                Name = "New Organisation",
                Description = "New Organisation",
                Website = "",
                //Avatar = 7_avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Organisation),
                MemberCount = 1
            };
        }

        public object BuildOrganisationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.GroupId)
                .Where(x => x.GroupType == "organisation")
                .Statistics(out stats)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
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
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(x.User.Id))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeOrganisation(All_Groups.Result result)
        {
            var organisationId = result.Organisation.Id.Replace("organisations/", "");

            return new
            {
                Id = organisationId,
                result.Organisation.Name,
                result.Organisation.Description,
                result.Organisation.Website,
                Avatar = result.Organisation.Avatar,
                MemberCount = result.DescendantGroupIds.Count()
                //Teams = result.DescendantGroups.Where(x => x.GroupType == "team").Select(x => x.Id),
                //Projects = result.DescendantGroups.Where(x => x.GroupType == "project").Select(x => x.Id)
            };
        }

        private object MakeUser(string userId)
        {
            // HACK: Massive N+1 problem right here
            return _userViewFactory.Make(_documentSession.Load<User>(userId));
        }

        #endregion
    }
}