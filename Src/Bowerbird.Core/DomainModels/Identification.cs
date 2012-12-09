using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Identification
    {

        #region Members

        #endregion

        #region Constructors

        protected Identification()
            : base()
        {
        }

        public Identification(
            bool isCustomIdentification,
            string category,
            string kingdom,
            string phylum,
            string className,
            string order,
            string family,
            string genus,
            string species,
            string subspecies,
            IEnumerable<string> commonGroupNames,
            IEnumerable<string> commonNames,
            IEnumerable<string> synonyms)
            : base()
        {
            // Can only check for kingdom as some Ids such as Funghi can be identified by kingdom alone
            Check.RequireNotNullOrWhitespace(category, "category");
            Check.RequireNotNullOrWhitespace(kingdom, "kingdom");
            Check.RequireNotNull(commonGroupNames, "commonGroupNames");
            Check.RequireNotNull(commonNames, "commonNames");
            Check.RequireNotNull(synonyms, "synonyms");

            InitMembers();

            IsCustomIdentification = isCustomIdentification;
            Category = category;

            SetTaxonomicRank(kingdom, "kingdom");
            SetTaxonomicRank(phylum, "phylum");
            SetTaxonomicRank(className, "class");
            SetTaxonomicRank(order, "order");
            SetTaxonomicRank(family, "family");
            SetTaxonomicRank(genus, "genus");
            SetTaxonomicRank(species, "species");
            SetTaxonomicRank(subspecies, "subspecies");

            RankName = TaxonomicRanks.Last().Name;
            RankType = TaxonomicRanks.Last().Type;

            if (TaxonomicRanks.Last().Type == "species" || TaxonomicRanks.Last().Type == "subspecies")
            {
                Name = (TryGetRankName("genus") + " " + TryGetRankName("species")).Trim();
            } 
            else
            {
                Name = TaxonomicRanks.Last().Name;
            }

            Taxonomy = string.Join(": ", TaxonomicRanks.Select(x => x.Name));

            CommonGroupNames = commonGroupNames;
            CommonNames = commonNames;
            Synonyms = synonyms;
        }

        #endregion

        #region Properties

        public bool IsCustomIdentification { get; private set; }

        public string Category { get; private set; }

        public string Name { get; private set; }

        public string RankName { get; private set; }

        public string RankType { get; private set; }

        public IEnumerable<string> CommonGroupNames { get; private set; }

        public IEnumerable<string> CommonNames { get; private set; }

        public string Taxonomy { get; private set; }

        public IEnumerable<TaxonomicRank> TaxonomicRanks { get; private set; }

        public IEnumerable<string> Synonyms { get; private set; }

        public string AllCommonNames
        {
            get { return string.Join(", ", CommonGroupNames.Concat(CommonNames)); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            TaxonomicRanks = new List<TaxonomicRank>();
        }

        private string TryGetRankName(string rankType)
        {
            if (TaxonomicRanks.Any(x => x.Type == rankType))
            {
                return TaxonomicRanks.Single(x => x.Type == rankType).Name;
            }
            return string.Empty;
        }

        private void SetTaxonomicRank(string name, string type)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                ((List<TaxonomicRank>)TaxonomicRanks).Add(new TaxonomicRank(name.Trim(), type));
            }
        }

        #endregion

    }
}
