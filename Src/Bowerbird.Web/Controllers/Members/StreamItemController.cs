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
using System.Linq.Expressions;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
				
namespace Bowerbird.Web.Controllers.Members
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public StreamItemController(
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            if(listInput.UserId != null)
            {
                return Json(MakeUserStreamItemList(listInput, sortInput));
            }

            if (listInput.GroupId != null)
            {
                return Json(MakeGroupStreamItemList(listInput, sortInput));
            }

            if (listInput.WatchlistId != null)
            {
                return Json(MakeWatchlistStreamItemList(listInput, sortInput));
            }

            return Json(MakeStreamItemList(listInput, sortInput));
        }

        /// <summary>
        /// Default User Stream
        /// </summary>
        private StreamItemList MakeStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .ToList();

            RavenQueryStatistics stats;
            IEnumerable<GroupContributionResults> groupContributions;

            if (sortInput.DateTimeDescending)
            {
                groupContributions = _documentSession
                    .Query<GroupContributionResults, All_GroupContributionItems>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                    .OrderByDescending(x => x.GroupCreatedDateTime)
                    .AsProjection<GroupContributionResults>()
                    .Statistics(out stats)
                    .Skip(listInput.Page)
                    .Take(listInput.PageSize)
                    .ToList();
            }
            else
            {
                groupContributions = _documentSession
                 .Query<GroupContributionResults, All_GroupContributionItems>()
                 .Include(x => x.ContributionId)
                 .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                 .OrderBy(x => x.GroupCreatedDateTime)
                 .AsProjection<GroupContributionResults>()
                 .Statistics(out stats)
                 .Skip(listInput.Page)
                 .Take(listInput.PageSize)
                 .ToList();
            }

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        private StreamItemList MakeGroupStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            IEnumerable<GroupContributionResults> groupContributions;

            if (sortInput.DateTimeDescending)
            {
                groupContributions = _documentSession
                    .Query<GroupContributionResults, All_GroupContributionItems>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                    .OrderByDescending(x => x.GroupCreatedDateTime)
                    .AsProjection<GroupContributionResults>()
                    .Statistics(out stats)
                    .Skip(listInput.Page)
                    .Take(listInput.PageSize)
                    .ToList();
            }
            else
            {
                groupContributions = _documentSession
                    .Query<GroupContributionResults, All_GroupContributionItems>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                    .OrderBy(x => x.GroupCreatedDateTime)
                    .AsProjection<GroupContributionResults>()
                    .Statistics(out stats)
                    .Skip(listInput.Page)
                    .Take(listInput.PageSize)
                    .ToList();
            }

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        // WORK IN PROGRESS... EARLY CHECKIN FOR REMOTE WORKING..
        private StreamItemList MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            IEnumerable<GroupContributionResults> groupContributions;

            if (sortInput.DateTimeDescending)
            {
                //groupContributions = _documentSession
                //    .Query<GroupContributionResults, All_GroupContributionItems>()
                //    .Include(x => x.ContributionId)
                //    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                //    .OrderByDescending(x => x.GroupCreatedDateTime)
                //    .AsProjection<GroupContributionResults>()
                //    .Statistics(out stats)
                //    .Skip(listInput.Page)
                //    .Take(listInput.PageSize)
                //    .ToList();

                /*
                var groupContributions = _documentSession
                    .Query<GroupContributionResults, All_GroupContributionItems>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));
                    
                    // Call BuildSort 

                    // Then continue on in here
                    groupContributions
                    .AsProjection<GroupContributionResults>()
                    .Statistics(out stats)
                    .Skip(listInput.Page)
                    .Take(listInput.PageSize)
                    .ToList();
                */

            }
            else
            {
                groupContributions = _documentSession
                    .Query<GroupContributionResults, All_GroupContributionItems>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                    .OrderBy(x => x.GroupCreatedDateTime)
                    .AsProjection<GroupContributionResults>()
                    .Statistics(out stats)
                    .Skip(listInput.Page)
                    .Take(listInput.PageSize)
                    .ToList();
            }

            /*
            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };
             * */

            //return streamItems;
            return null;
        }

        //private void BuildSort(IQueryable<GroupContributionResults> query, Queryinput)
        //{
        //    query.OrderByDescending(x => x.GroupCreatedDateTime);
        //}

        private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            var groupContributions = _documentSession
                .Query<GroupContributionResults, All_GroupContributionItems>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                .OrderByDescending(x => x.GroupCreatedDateTime)
                .AsProjection<GroupContributionResults>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        #endregion
    }
}