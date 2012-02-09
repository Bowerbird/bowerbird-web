/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeHomeIndex(homeIndexInput));
            }
            return View(MakeHomeIndex(homeIndexInput));
        }

        private HomeIndex MakeHomeIndex(HomeIndexInput indexInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Where(x => x.User.Id == indexInput.UserId)
                .ToList();

            RavenQueryStatistics stats;

            var groupContributions = _documentSession
                .Query<GroupContributionResults, All_GroupContributionItems>()
                //.Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                .Statistics(out stats)
                .Skip(indexInput.Page)
                .Take(indexInput.PageSize)
                .ToList();

            //var contributions = _documentSession.Query<Contribution>()
            //    .Where(x => x.Id.In(groupContributions.Select(y => y.ContributionId)))
            //    .Statistics(out stats)
            //    .Skip(indexInput.Page)
            //    .Take(indexInput.PageSize)
            //    .ToList();

            var user = _documentSession.Load<User>(indexInput.UserId);

            var homeIndex = new HomeIndex()
            {
                ProjectMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                TeamMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                UserProfile =
                    new UserProfile()
                    {
                        Id = user.Id,
                        Name = string.Format("{0} {1}", user.FirstName, user.LastName)
                    },

                WatchlistMenu = _documentSession
                .Query<Watchlist>()
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.QuerystringJson,
                        Name = x.Name
                    })
                    .ToList()
                ,

                StreamItems = groupContributions
                .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        //SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }).ToPagedList(
                    indexInput.Page,
                    indexInput.PageSize,
                    stats.TotalResults,
                    null)

                //StreamItems = contributions
                //.Select(x =>
                //    new StreamItemViewModel()
                //    {
                //        Item = x,
                //        ItemId = x.Id,
                //        SubmittedOn = x.CreatedOn,
                //        UserId = x.User.Id
                //    }).ToPagedList(
                //    indexInput.Page,
                //    indexInput.PageSize,
                //    stats.TotalResults,
                //    null)
            };

            return homeIndex;
        }

        //private HomeIndex MakeHomeIndex(HomeIndexInput indexInput)
        //{
        //    var groupMemberships = _documentSession
        //        .Query<GroupMember>()
        //        .Where(x => x.User.Id == indexInput.UserId)
        //        .ToList();

        //    RavenQueryStatistics stats;

        //    var groupContributions = _documentSession
        //        .Query<GroupContribution>()
        //        .Include(x => x.Contribution.Id)
        //        .OrderByDescending(x => x.CreatedDateTime)//;
        //        .Statistics(out stats)
        //        .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
        //        .Skip(indexInput.Page)
        //        .Take(indexInput.PageSize)
        //        .ToList();

        //    //var contributions = groupContributions.
        //        //.Query<Contribution, All_Contributions>()
        //        //.Query<Contribution>()
        //        //.Where(x => x.Id.In(groupContributions.Select(y => y.ContributionId)))
        //        //.OrderBy(x = > x.CreatedOn)
        //        //.ToList();

        //    var user = _documentSession.Load<User>(indexInput.UserId);

        //    var homeIndex = new HomeIndex()
        //    {
        //        ProjectMenu = groupMemberships
        //        .Where(x => x.Group.Id.Contains("projects/"))
        //        .Select(x =>
        //            new MenuItem()
        //            {
        //                Id = x.Group.Id,
        //                Name = x.Group.Name
        //            }
        //        ),

        //        TeamMenu = groupMemberships
        //        .Where(x => x.Group.Id.Contains("teams/"))
        //        .Select(x =>
        //            new MenuItem()
        //            {
        //                Id = x.Group.Id,
        //                Name = x.Group.Name
        //            }
        //        ),

        //        UserProfile =
        //            new UserProfile()
        //            {
        //                Id = user.Id,
        //                Name = string.Format("{0} {1}", user.FirstName, user.LastName)
        //            },

        //        WatchlistMenu = _documentSession
        //        .Query<Watchlist>()
        //        .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
        //        .Select(x =>
        //            new MenuItem()
        //            {
        //                Id = x.QuerystringJson,
        //                Name = x.Name
        //            })
        //        ,

        //        StreamItems = contributions
        //        .Select(x =>
        //            new StreamItemViewModel()
        //            {
        //                Item = x,
        //                ItemId = x.Id,
        //                SubmittedOn = x.CreatedOn,
        //                UserId = x.User.Id
        //            }).ToPagedList(
        //            indexInput.Page,
        //            indexInput.PageSize,
        //            stats.TotalResults,
        //            null)
        //    };

        //    return homeIndex;
        //}

        #endregion      
    }
}