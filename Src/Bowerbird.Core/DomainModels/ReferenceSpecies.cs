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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class ReferenceSpecies
    {
        #region Fields

        private IEnumerable<string> _smartTags { get; set; }

        #endregion

        #region Constructors

        protected ReferenceSpecies()
        {
            InitMembers();
        }
    
        public ReferenceSpecies(
            DateTime createdOn,
            Species species,
            IEnumerable<string> smartTags)
            : base()
        {
            Check.RequireNotNull(species, "species");

            InitMembers();

            CreatedDateTime = createdOn;
            SpeciesId = species.Id;

            SetDetails(smartTags);
        }

        #endregion

        #region Properties

        public DateTime CreatedDateTime { get; private set; }

        public DateTime UpdatedDateTime { get; private set; }

        public string SpeciesId { get; private set; }

        public IEnumerable<string> SmartTags { get { return _smartTags; } }
        
        #endregion

        #region Methods

        private void InitMembers()
        {
            _smartTags = new List<string>();
        }

        private void SetDetails(IEnumerable<string> smartTags)
        {
            _smartTags = smartTags;
            UpdatedDateTime = DateTime.UtcNow;
        }

        public void UpdateDetails(IEnumerable<string> smartTags)
        {
            SetDetails(smartTags);
        }

        #endregion
    }
}