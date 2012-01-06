using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Permission : DomainModel, INamedDomainModel
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