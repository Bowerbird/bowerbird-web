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
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserJoinedGroup : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Member>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ActivityUserJoinedGroup(
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

        public void Handle(DomainModelCreatedEvent<Member> domainEvent)
        {
            var user = _documentSession.Load<User>(domainEvent.DomainModel.User.Id);
            var group = _documentSession.Load<dynamic>(domainEvent.DomainModel.Group.Id);

            dynamic activity = MakeActivity(
                domainEvent, 
                "userjoinedgroup", 
                string.Format("{0} joined {1}", user.FirstName, group.Name), 
                new[] { domainEvent.DomainModel.Group });

            activity.UserJoinedGroup = new
            {
                User = user,
                Group = group
            };

            _documentSession.Store(activity);
            _userContext.SendActivityToGroupChannel(activity);
        }

        #endregion
    }
}