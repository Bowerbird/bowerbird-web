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
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class Role : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected Role()
            : base()
        {
            InitMembers();

            EnableEvents();
        }

        public Role(
            string id,
            string name,
            string description,
            IEnumerable<Permission> permissions)
            : base()
        {
            Check.RequireNotNullOrWhitespace(id, "id");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");
            Check.RequireNotNull(permissions, "permissions");

            InitMembers();

            Id = "roles/" + id;
            Name = name;
            Description = description;

            Permissions = permissions.ToList();

            EnableEvents();
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IEnumerable<Permission> Permissions { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Permissions = new List<Permission>();
        }

        #endregion

    }
}