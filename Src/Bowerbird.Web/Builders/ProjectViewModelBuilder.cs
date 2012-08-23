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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Factories;
using System.Collections.Generic;

namespace Bowerbird.Web.Builders
{
    public class ProjectViewModelBuilder : IProjectViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public ProjectViewModelBuilder(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
        }

        #endregion

        #region Methods

        public object BuildNewProject()
        {
            return new
            {
                Name = string.Empty,
                Description = string.Empty,
                Website = string.Empty,
                Avatar = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Project),
                MemberCount = 1
            };
        }

        public object BuildProject(string projectId)
        {
            Check.RequireNotNullOrWhitespace(projectId, "projectId");

            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == projectId);

            return _groupViewFactory.Make(project);
        }

        public object BuildUserProjectList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "project" && x.UserIds.Any(y => y == userId))
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_groupViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        public object BuildGroupProjectList(string groupId, bool getAllDescendants, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>();

            if (groupId == Constants.AppRootId)
            {
                if (getAllDescendants)
                {
                    query = query
                        .Where(x => x.GroupType == "project");
                }
                else
                {
                    query = query
                        .Where(x => x.GroupType == "project" && x.ParentGroupId == Constants.AppRootId);
                }
            }
            else
            {
                var group = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>()
                    .Where(x => x.GroupId == groupId)
                    .ToList()
                    .First();

                var descendantGroupIds = getAllDescendants ? group.DescendantGroupIds : group.ChildGroupIds;

                query = query
                    .Where(x => x.GroupId.In(descendantGroupIds) && x.GroupType == "project");
            }

            RavenQueryStatistics stats;

            return query
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_groupViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}