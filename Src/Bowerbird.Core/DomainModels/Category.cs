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
    public class Category
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
            string name,
            IEnumerable<string> rootTaxonomies)
            : base()
        {
            Check.RequireNotNullOrWhitespace(id, "id");

            Id = id;
            Name = name;
            RootTaxonomies = rootTaxonomies;
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<string> RootTaxonomies { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}