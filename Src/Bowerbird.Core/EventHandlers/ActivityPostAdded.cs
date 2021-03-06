﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityPostAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Post>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityPostAdded(
            IDocumentSession documentSession,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Post> domainEvent)
        {
            var group = _documentSession.Load<dynamic>(domainEvent.DomainModel.Group.Id);
            
            dynamic activity = MakeActivity(
                domainEvent, 
                "postadded",
                domainEvent.DomainModel.CreatedOn,
                string.Format("{0} added a news item", domainEvent.User.GetName(), ((Group)group).Name, ((Group)group).GroupType), 
                new List<dynamic>(){group},
                domainEvent.DomainModel.Id);

            activity.PostAdded = new
            {
                Post = domainEvent.DomainModel
            };

            _documentSession.Store(activity);
            _documentSession.SaveChanges();
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}