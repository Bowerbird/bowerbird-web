using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Permission : Entity, INamedEntity
    {

        #region Members

        #endregion

        #region Constructors

        protected Permission() : base() {}

        public Permission(
            string id,
            string name,
            string description)
            : this()
        {
            Check.RequireNotNullOrWhitespace(id, "id");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                id,
                name,
                description);
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string id, string name, string description)
        {
            Id = "permissions/" + id;
            Name = name;
            Description = description;
        }

        #endregion

    }
}