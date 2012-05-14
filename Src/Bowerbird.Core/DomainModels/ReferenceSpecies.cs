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
    public class ReferenceSpecies : DomainModel
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
            User createdByUser,
            DateTime createdOn,
            string groupId,
            string speciesId,
            IEnumerable<string> smartTags
            )
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNullOrWhitespace(speciesId, "speciesId");

            InitMembers();

            User = createdByUser;
            CreatedDateTime = createdOn;

            SetDetails(
                groupId,
                speciesId,
                smartTags
                );
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string GroupId { get; set; }
        
        public string SpeciesId { get; set; }

        public IEnumerable<string> SmartTags { get { return _smartTags; } }
        
        #endregion

        #region Methods

        private void InitMembers()
        {
            _smartTags = new List<string>();
        }

        private void SetDetails(
            string groupId,
            string speciesId,
            IEnumerable<string> smartTags
            )
        {
            GroupId = groupId;
            SpeciesId = speciesId;
            _smartTags = smartTags;
        }

        public void UpdateDetails(
            string groupId,
            string speciesId,
            IEnumerable<string> smartTags
            )
        {
            SetDetails(
                groupId,
                speciesId,
                smartTags
                );
        }

        #endregion
    }
}