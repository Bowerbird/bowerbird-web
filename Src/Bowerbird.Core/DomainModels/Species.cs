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
using System.Linq;
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
            InitMembers();
        }

        public Species(
            string category,
            IEnumerable<string> commonGroupNames,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName,
            string subSpeciesName,
            string synonym,
            DateTime createdOn,
            User createdByUser)
            : base()
        {
            InitMembers();

            CreatedDateTime = createdOn;
            CreatedByUser = createdByUser;

            SetSpeciesDetails(
                category,
                commonGroupNames,
                commonNames,
                kingdomName,
                phylumName,
                className,
                orderName,
                familyName,
                genusName,
                speciesName,
                subSpeciesName,
                synonym);

            ApplyEvent(new DomainModelCreatedEvent<Species>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Category { get; private set; }

        public IEnumerable<string> CommonGroupNames { get; private set; }

        public IEnumerable<string> CommonNames { get; private set; }

        public IEnumerable<TaxonomicRank> Taxonomy { get; private set; }

        public string Synonym { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public string KingdomName { get { return TryGetRankName("kingdom"); } }

        public string PhylumName { get { return TryGetRankName("phylum"); } }

        public string ClassName { get { return TryGetRankName("class"); } }

        public string OrderName { get { return TryGetRankName("order"); } }

        public string FamilyName { get { return TryGetRankName("family"); } }

        public string GenusName { get { return TryGetRankName("genus"); } }

        public string SpeciesName { get { return TryGetRankName("species"); } }

        public string SubSpeciesName { get { return TryGetRankName("subspecies"); } }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Taxonomy = new List<TaxonomicRank>();
        }

        private string TryGetRankName(string rankType)
        {
            if (Taxonomy.Any(x => x.Type == rankType))
            {
                return Taxonomy.Single(x => x.Type == rankType).Name;
            }
            return string.Empty;
        }

        private void SetTaxonomicRank(string name, string type, List<TaxonomicRank> taxonomicRanks)
        {
            if (!string.IsNullOrWhiteSpace(name)) 
            {
                taxonomicRanks.Add(new TaxonomicRank(name.Trim(), type));
            }
        }

        public void SetSpeciesDetails(
            string category,
            IEnumerable<string> commonGroupNames,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName,
            string subSpeciesName,
            string synonym
            )
        {
            Category = category;
            CommonGroupNames = commonGroupNames;
            CommonNames = commonNames;

            var newTaxonomicRanks = new List<TaxonomicRank>();

            SetTaxonomicRank(kingdomName, "kingdom", newTaxonomicRanks);
            SetTaxonomicRank(phylumName, "phylum", newTaxonomicRanks);
            SetTaxonomicRank(className, "class", newTaxonomicRanks);
            SetTaxonomicRank(orderName, "order", newTaxonomicRanks);
            SetTaxonomicRank(familyName, "family", newTaxonomicRanks);
            SetTaxonomicRank(genusName, "genus", newTaxonomicRanks);
            SetTaxonomicRank(speciesName, "species", newTaxonomicRanks);
            SetTaxonomicRank(subSpeciesName, "subspecies", newTaxonomicRanks);
            
            Taxonomy = newTaxonomicRanks;

            Synonym = synonym;
        }

        public void UpdateDetails(
            string category,
            IEnumerable<string> commonGroupNames,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName,
            string subSpeciesName,
            string synonym,
            User updatedByUser
            )
        {
            SetSpeciesDetails(
                category,
                commonGroupNames,
                commonNames,
                kingdomName,
                phylumName,
                className,
                orderName,
                familyName,
                genusName,
                speciesName,
                subSpeciesName,
                synonym);

            ApplyEvent(new DomainModelUpdatedEvent<Species>(this, updatedByUser, this));
        }

        #endregion
    }
}