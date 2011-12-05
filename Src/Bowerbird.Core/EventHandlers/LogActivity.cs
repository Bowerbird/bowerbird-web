using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.EventHandlers
{
    public class LogActivity : IEventHandler<EntityCreatedEvent<User>>, IEventHandler<EntityCreatedEvent<Observation>>
    {

        #region Members

        private readonly IRepository<Activity> _activityRepository;

        #endregion

        #region Constructors

        public LogActivity(
            IRepository<Activity> activityRepository)
        {
            Check.RequireNotNull(activityRepository, "activityRepository");

            _activityRepository = activityRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(EntityCreatedEvent<User> userCreatedEvent)
        {
            SaveEntityCreatedActivity(userCreatedEvent);
        }

        public void Handle(EntityCreatedEvent<Observation> observationCreatedEvent)
        {
            SaveEntityCreatedActivity(observationCreatedEvent);
        }

        private void SaveEntityCreatedActivity<T>(EntityCreatedEvent<T> entityCreatedEvent) where T : IEntity
        {
            Check.RequireNotNull(entityCreatedEvent, "entityCreatedEvent");

            SaveActivity(
                entityCreatedEvent.Entity.GetType().Name.ToLower() + "created",
                entityCreatedEvent.CreatedByUser,
                entityCreatedEvent.Entity);
        }

        private void SaveActivity(string type, User user, object data)
        {
            var activity = new Activity(
                type,
                user,
                data);

            //_activityRepository.Add(activity);

            //Console.Write("logging activity type: {0} id: {1}", type, activity.Id);
        }

        #endregion

    }
}
