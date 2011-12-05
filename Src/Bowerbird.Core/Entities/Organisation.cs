using Bowerbird.Core.DesignByContract;
using System;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Organisation : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected Organisation()
            : base()
        {
        }

        public Organisation(
            User createdByUser,
            string name,
            string description,
            string website)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new EntityCreatedEvent<Organisation>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Website { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string name, string description, string website)
        {
            Name = name;
            Description = description;
            Website = website;
        }

        public Organisation UpdateDetails(User updatedByUser, string name, string description, string website)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new EntityUpdatedEvent<Organisation>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}