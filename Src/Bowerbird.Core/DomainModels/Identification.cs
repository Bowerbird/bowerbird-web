using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class IdentificationNew : ISubContribution
    {

        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Vote> _votes;

        #endregion

        #region Constructors

        protected IdentificationNew()
            : base()
        {
            InitMembers();
        }

        public IdentificationNew(
            int id,
            User createdByUser,
            DateTime createdOn,
            string comments,
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
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(category, "category");
            Check.RequireNotNullOrWhitespace(kingdom, "kingdom"); // Can only check for kingdom as some Ids such as Funghi can be identified by kingdom alone
            Check.RequireNotNull(commonGroupNames, "commonGroupNames");
            Check.RequireNotNull(commonNames, "commonNames");
            Check.RequireNotNull(synonyms, "synonyms");

            InitMembers();

            SequentialId = id;
            Id = "identifications/" + id.ToString();
            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                comments,
                isCustomIdentification,
                category,
                kingdom,
                phylum,
                className,
                order,
                family,
                genus, 
                species,
                subspecies,
                commonGroupNames,
                commonNames,
                synonyms);
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public int SequentialId { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public string Comments { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public DateTime UpdateOn { get; private set; }

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

        public IEnumerable<Vote> Votes
        {
            get { return _votes; }
            private set { _votes = new List<Vote>(value); }
        }

        public string AllCommonNames
        {
            get { return string.Join(", ", CommonGroupNames.Concat(CommonNames)); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            TaxonomicRanks = new List<TaxonomicRank>();
            _votes = new List<Vote>();
        }

        public string TryGetRankName(string rankType)
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

        private void SetDetails(
            string comments,
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
        {
            Comments = comments;
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

        public IdentificationNew UpdateDetails(
            User updatedByUser,
            DateTime updatedOn,
            string comments,
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
        {
            UpdateOn = updatedOn;

            SetDetails(
                comments,
                isCustomIdentification,
                category,
                kingdom,
                phylum,
                className,
                order,
                family,
                genus,
                species,
                subspecies,
                commonGroupNames,
                commonNames,
                synonyms);

            return this;
        }

        private Vote SetVote(
            int id,
            int score,
            DateTime createdOn,
            User createdByUser)
        {
            var vote = new Vote(
                id,
                createdByUser,
                score,
                createdOn);

            _votes.Add(vote);

            return vote;
        }

        public Vote UpdateVote(
            int score,
            DateTime createdOn,
            User createdByUser)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            _votes.RemoveAll(x => x.User.Id == createdByUser.Id);

            if (score != 0)
            {
                var maxId = _votes.Count > 0 ? _votes.Select(x => x.SequentialId).Max() : 0;

                Vote vote = SetVote(maxId + 1, score, createdOn, createdByUser);

                return vote;
            }

            return null;
        }

        public ISubContribution GetSubContribution(string type, string id)
        {
            return null;
        }

        #endregion

    }
}
