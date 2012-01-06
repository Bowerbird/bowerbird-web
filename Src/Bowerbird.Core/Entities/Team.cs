using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Team : Entity, INamedEntity
    {

        #region Members

        #endregion

        #region Constructors

        protected Team() : base() {}

        public Team(
            User createdByUser,
            string name,
            string description,
            string website)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new EntityCreatedEvent<Team>(this, createdByUser));
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

        public Team UpdateDetails(User updatedByUser, string name, string description, string website)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new EntityUpdatedEvent<Team>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}