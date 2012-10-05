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
    public class Category : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected Category()
            : base()
        {
        }

        public Category(
            string id,
            string taxonomy)
            : base()
        {
            Check.RequireNotNullOrWhitespace(id, "id");
            Check.RequireNotNullOrWhitespace(taxonomy, "taxonomy");

            Id = "categories/" + id;
            Name = id;
            Taxonomy = taxonomy;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Taxonomy { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}