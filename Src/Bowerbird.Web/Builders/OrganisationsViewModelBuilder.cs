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
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public OrganisationsViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _avatarFactory = avatarFactory;
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
                Avatar = _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Organisation),
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
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.GroupIds.Any(yield => yield == pagingInput.Id))
                .Statistics(out stats)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _userViewFactory.Make(x.User))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeOrganisation(All_Groups.Result result)
        {
            return new
            {
                result.Organisation.Id,
                result.Organisation.Name,
                result.Organisation.Description,
                result.Organisation.Website,
                result.Organisation.Avatar,
                MemberCount = result.DescendantGroupIds.Count(),
                AvatarId = result.Organisation.Avatar != null ? result.Organisation.Avatar.Id : null
            };
        }

        #endregion
    }
}