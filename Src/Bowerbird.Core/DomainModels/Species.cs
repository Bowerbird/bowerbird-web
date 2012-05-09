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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Species : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        protected Species()
            : base()
        {
            InitMembers();
        }

        public Species(
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            bool proposedAsNewSpecies,
            DateTime createdOn
            )
        {
            InitMembers();

            CreatedDateTime = createdOn;

            SetDetails(
                kingdom,
                group,
                commonNames,
                taxonomy,
                order,
                family,
                genusName,
                speciesName,
                proposedAsNewSpecies
                );
        }

        public Species(
            User createdByUser,
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            bool proposedAsNewSpecies,
            DateTime createdOn
            )
        {
            InitMembers();

            CreatedByUser = createdByUser;
            CreatedDateTime = createdOn;

            SetDetails(
                kingdom,
                group,
                commonNames,
                taxonomy,
                order,
                family,
                genusName,
                speciesName,
                proposedAsNewSpecies
                );
        }

        #endregion

        #region Properties

        public string Kingdom { get; private set; }

        public string Group { get; private set; }

        public IEnumerable<string> CommonNames { get; private set; }

        public string Taxonomy { get; private set; }

        public string Order { get; private set; }

        public string Family { get; private set; }

        public string GenusName { get; private set; }

        public string SpeciesName { get; private set; }

        public bool ProposedAsNewSpecies { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; set; }

        public DenormalisedUserReference ProposedByUser { get; set; }

        public DenormalisedUserReference EndorsedByUser { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {

        }

        public void SetDetails(
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            bool proposedAsNewSpecies
            )
        {
            Kingdom = kingdom;
            Group = group;
            CommonNames = commonNames;
            Taxonomy = taxonomy;
            Order = order;
            Family = family;
            GenusName = genusName;
            SpeciesName = speciesName;
            ProposedAsNewSpecies = proposedAsNewSpecies;
        }

        public void UpdateDetails(
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            bool proposedAsNewSpecies
            )
        {
            Kingdom = kingdom;
            Group = group;
            CommonNames = commonNames;
            Taxonomy = taxonomy;
            Order = order;
            Family = family;
            GenusName = genusName;
            SpeciesName = speciesName;
            ProposedAsNewSpecies = proposedAsNewSpecies;
        }

        #endregion
    }
}