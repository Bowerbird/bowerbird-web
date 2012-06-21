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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using System.Dynamic;
using System.IO;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.EventHandlers
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
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ActivityPostAdded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Post> domainEvent)
        {
            var group = _documentSession.Load<dynamic>(domainEvent.DomainModel.GroupId);
            
            dynamic activity = MakeActivity(
                domainEvent, 
                "postadded", 
                string.Format("{0} added a post to the {1} {2}", domainEvent.User.GetName(), ((Group)group).Name, ((Group)group).GroupType), 
                group);

            activity.PostAdded = new
            {
                Post = domainEvent.DomainModel
            };

            _documentSession.Store(activity);
            _userContext.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}