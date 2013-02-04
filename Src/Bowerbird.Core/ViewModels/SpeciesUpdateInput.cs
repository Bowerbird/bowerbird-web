/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;

namespace Bowerbird.Core.ViewModels
{
    public class SpeciesUpdateInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Kingdom { get; set; }

        public string Group { get; set; }

        public IEnumerable<string> CommonNames { get; set; }

        public string Taxonomy { get; set; }

        public string Order { get; set; }

        public string Family { get; set; }

        public string GenusName { get; set; }

        public string SpeciesName { get; set; }

        public bool ProposedAsNewSpecies { get; set; }

        public string UserId { get; set; }

        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}