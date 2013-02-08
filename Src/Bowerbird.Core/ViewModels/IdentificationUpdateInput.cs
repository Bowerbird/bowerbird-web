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
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Validators;

namespace Bowerbird.Core.ViewModels
{
    public class IdentificationUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Used when adding an identification at the same time as a new sighting only.
        /// </summary>
        public bool NewSighting { get; set; }

        public int? Id { get; set; }

        public string SightingId { get; set; }

        public string IdentificationComments { get; set; }

        /// <summary>
        /// A custom identification consist of all taxa filled out. A non-custom one consists of just Taxonomy, 
        /// which is then used to source the identification from our classification index.
        /// </summary>
        [IdentificationRequired(ErrorMessageResourceName = "IdentificationRequired", ErrorMessageResourceType = typeof(I18n))]
        public bool IsCustomIdentification { get; set; }

        #region Non-custom Identification

        public string Taxonomy { get; set; }

        #endregion

        #region Custom Identification

        public string Category { get; set; }

        public string Kingdom { get; set; }

        public string Phylum { get; set; }

        public string Class { get; set; }

        public string Order { get; set; }

        public string Family { get; set; }

        public string Genus { get; set; }

        public string Species { get; set; }

        public string Subspecies { get; set; }

        public IEnumerable<string> CommonGroupNames { get; set; }

        public IEnumerable<string> CommonNames { get; set; }

        public IEnumerable<string> Synonyms { get; set; }

        #endregion

        #endregion

        #region Methods

        #endregion      
    }
}