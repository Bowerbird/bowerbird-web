using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Bowerbird.Core.Entities
{
    public class Role : Entity, INamedEntity
    {

        #region Members

        #endregion

        #region Constructors

        protected Role() : base() {}

        public Role(
            string id,
            string name,
            string description,
            IEnumerable<Permission> permissions)
            : this()
        {
            Check.RequireNotNullOrWhitespace(id, "id");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");
            Check.RequireNotNull(permissions, "permissions");

            SetDetails(   
                id,
                name,
                description,
                permissions);
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public List<DenormalisedNamedEntityReference<Permission>> Permissions { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string id, string name, string description, IEnumerable<Permission> permissions)
        {
            Id = "roles/" + id;
            Name = name;
            Description = description;

            Permissions = permissions.Select(permission => {
                DenormalisedNamedEntityReference<Permission> denorm = permission;
                return denorm;
            }).ToList();
        }

        #endregion

    }
}