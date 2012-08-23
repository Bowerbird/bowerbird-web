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
using Bowerbird.Core.Events;

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
        }

        public Species(
            string category,
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            string synonym,
            bool proposedAsNewSpecies,
            DateTime createdOn)
            : base()
        {
            CreatedDateTime = createdOn;

            SetSpeciesDetails(
                category,
                kingdom,
                group,
                commonNames,
                taxonomy,
                order,
                family,
                genusName,
                speciesName,
                synonym,
                proposedAsNewSpecies
                );
        }

        public Species(
            User createdByUser,
            string category,
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            string synonym,
            bool proposedAsNewSpecies,
            DateTime createdOn)
            : base()
        {
            CreatedByUser = createdByUser;
            CreatedDateTime = createdOn;

            SetSpeciesDetails(
                category,
                kingdom,
                group,
                commonNames,
                taxonomy,
                order,
                family,
                genusName,
                speciesName,
                synonym,
                proposedAsNewSpecies
                );

            ApplyEvent(new DomainModelCreatedEvent<Species>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Category { get; private set; }

        public string Kingdom { get; private set; }

        public string Group { get; private set; }

        public IEnumerable<string> CommonNames { get; private set; }

        public string Taxonomy { get; private set; }

        public string Order { get; private set; }

        public string Family { get; private set; }

        public string GenusName { get; private set; }

        public string SpeciesName { get; private set; }

        public string Synonym { get; private set; }

        public bool ProposedAsNewSpecies { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DenormalisedUserReference ProposedByUser { get; private set; }

        public DenormalisedUserReference EndorsedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public DateTime ModifiedDateTime { get; private set; }

        #endregion

        #region Methods

        public void SetSpeciesDetails(
            string category,
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            string synonym,
            bool proposedAsNewSpecies
            )
        {
            Category = category;
            Kingdom = kingdom;
            Group = group;
            CommonNames = commonNames;
            Taxonomy = taxonomy;
            Order = order;
            Family = family;
            GenusName = genusName;
            SpeciesName = speciesName;
            Synonym = synonym;
            ProposedAsNewSpecies = proposedAsNewSpecies;
        }

        public void UpdateDetails(
            string category,
            string kingdom,
            string group,
            IEnumerable<string> commonNames,
            string taxonomy,
            string order,
            string family,
            string genusName,
            string speciesName,
            string synonym,
            bool proposedAsNewSpecies,
            User updatedByUser
            )
        {
            SetSpeciesDetails(
                category,
                kingdom,
                group,
                commonNames,
                taxonomy,
                order,
                family,
                genusName,
                speciesName,
                synonym,
                proposedAsNewSpecies);

            ApplyEvent(new DomainModelUpdatedEvent<Species>(this, updatedByUser, this));
        }

        #endregion
    }
}