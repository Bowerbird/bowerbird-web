using Bowerbird.Core.DesignByContract;
using System;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Project : Entity, INamedEntity
    {

        #region Members

        #endregion

        #region Constructors

        protected Project()
            : base()
        {
        }

        public Project(
            User createdByUser,
            string name,
            string description)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            SetDetails(
                name,
                description);

            EventProcessor.Raise(new EntityCreatedEvent<Project>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Project UpdateDetails(User updatedByUser, string name, string description)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                name,
                description);

            EventProcessor.Raise(new EntityUpdatedEvent<Project>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}