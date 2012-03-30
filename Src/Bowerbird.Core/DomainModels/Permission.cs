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

            Id = "permissions/" + id;
            Name = name;
            Description = description;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}