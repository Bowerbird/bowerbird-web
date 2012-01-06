using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Project : DomainModel, INamedDomainModel
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
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description);

            EventProcessor.Raise(new DomainModelCreatedEvent<Project>(this, createdByUser));
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
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Project>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}