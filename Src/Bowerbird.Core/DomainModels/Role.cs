/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class Role : DomainModel, INamedDomainModel
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

        public List<DenormalisedNamedDomainModelReference<Permission>> Permissions { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string id, string name, string description, IEnumerable<Permission> permissions)
        {
            Id = "roles/" + id;
            Name = name;
            Description = description;

            Permissions = permissions.Select(permission => {
                DenormalisedNamedDomainModelReference<Permission> denorm = permission;
                return denorm;
            }).ToList();
        }

        #endregion

    }
}