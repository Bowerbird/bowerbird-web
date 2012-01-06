using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Organisation : DomainModel
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
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new DomainModelCreatedEvent<Organisation>(this, createdByUser));
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
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}