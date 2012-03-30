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
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Role : DomainModel, INamedDomainModel
    {

        #region Members

        [JsonIgnore]
        private List<DenormalisedNamedDomainModelReference<Permission>> _permissions;

        #endregion

        #region Constructors

        protected Role()
            : base()
        {
            InitMembers();
        }

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

            Id = "roles/" + id;
            Name = name;
            Description = description;

            Permissions = permissions.Select(permission =>
            {
                DenormalisedNamedDomainModelReference<Permission> denorm = permission;
                return denorm;
            }).ToList();
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IEnumerable<DenormalisedNamedDomainModelReference<Permission>> Permissions 
        {
            get { return _permissions; }
            private set { _permissions = new List<DenormalisedNamedDomainModelReference<Permission>>(value); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _permissions = new List<DenormalisedNamedDomainModelReference<Permission>>();
        }

        #endregion

    }
}