/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class TaxonomicRank
    {
        #region Members

        #endregion

        #region Constructors

        protected TaxonomicRank()
            : base()
        {
        }

        public TaxonomicRank(
            string name,
            string type)
            : base()
        {
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(type, "type");

            Name = name;
            Type = type;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Type { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}