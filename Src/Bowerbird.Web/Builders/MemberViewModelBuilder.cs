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
    public class MemberViewModelBuilder : IMemberViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMemberViewFactory _memberViewFactory;

        #endregion

        #region Constructors

        public MemberViewModelBuilder(
            IDocumentSession documentSession,
            IMemberViewFactory memberViewFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(memberViewFactory, "memberViewFactory");

            _documentSession = documentSession;
            _memberViewFactory = memberViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildMember(IdInput idInput)
        {
            return _memberViewFactory.Make(_documentSession.Load<Member>(idInput.Id));
        }

        public object BuildProjectMemberList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _memberViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Users = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildTeamMemberList(PagingInput pagingInput)
        {
            var projectsInTeam = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == pagingInput.Id)
                .Include(x => x.ChildGroupId)
                .Select(x => x.ChildGroupId)
                .ToList();

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.GroupId.In(projectsInTeam.Intersect(new List<string>() {pagingInput.Id})))
                .Select(x => x.Member)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _memberViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Users = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildOrganisationMemberList(PagingInput pagingInput)
        {
            var teamsInOrganisation = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == pagingInput.Id)
                .Include(x => x.ChildGroupId)
                .Select(x => x.ChildGroupId)
                .ToList();

            var projectsInTeams = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId.In(teamsInOrganisation))
                .Include(x => x.ChildGroupId)
                .Select(x => x.ChildGroupId)
                .ToList();

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.GroupId.In(projectsInTeams.Intersect(new List<string>() { pagingInput.Id })))
                .Select(x => x.Member)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _memberViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Users = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}