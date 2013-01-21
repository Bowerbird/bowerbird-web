/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class SightingNote : ISubContribution
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private IEnumerable<SightingNoteDescription> _descriptions;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore] 
        private IEnumerable<string> _tags;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Vote> _votes;

        #endregion

        #region Constructors

        protected SightingNote() 
            : base() 
        {
            InitMembers();
        }

        public SightingNote(
            int id,
            User createdByUser,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            string comments,
            DateTime createdOn)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(tags, "tags");

            SequentialId = id;
            Id = "notes/" + id.ToString();
            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                tags,
                descriptions,
                comments);
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public int SequentialId { get; private set; }
        
        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
            private set { _tags = new List<string>(value); }
        }

        public IEnumerable<SightingNoteDescription> Descriptions 
        {
            get { return _descriptions; }
            private set { _descriptions = new List<SightingNoteDescription>(value); }
        }

        public IEnumerable<Vote> Votes
        {
            get { return _votes; }
            private set { _votes = new List<Vote>(value); }
        }

        public string Comments { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _descriptions = new List<SightingNoteDescription>();
            _tags = new List<string>();
            _votes = new List<Vote>();
        }

        protected void SetDetails(
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions,
            string comments)
        {
            Tags = tags;
            Descriptions = descriptions.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => MakeSightingNoteDescription(x.Key, x.Value));
            Comments = comments;
        }

        public SightingNote UpdateDetails(
            User updatedByUser, 
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions,
            string comments)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(descriptions, "descriptions");

            SetDetails(
                tags,
                descriptions,
                comments);

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

        private SightingNoteDescription MakeSightingNoteDescription(string id, string text)
        {
            string label = string.Empty;
            string description = string.Empty;
            string group = string.Empty;

            switch (id)
            {
                case "physicaldescription":
                    group = "lookslike";
                    label = "Physical Description";
                    description = "The physical characteristics of the species in the sighting";
                    break;
                case "similarspecies":
                    group = "lookslike";
                    label = "Similar Species";
                    description = "How the species sighting is similar to other species";
                    break;
                case "distribution":
                    group = "wherefound";
                    label = "Distribution";
                    description = "The geographic distribution of the species in the sighting";
                    break;
                case "habitat":
                    group = "wherefound";
                    label = "Habitat";
                    description = "The habitat of the species in the sighting";
                    break;
                case "seasonalvariation":
                    group = "wherefound";
                    label = "Seasonal Variation";
                    description = "Any seasonal variation of the species in the sighting";
                    break;
                case "conservationstatus":
                    group = "wherefound";
                    label = "Conservation Status";
                    description = "The conservation status of the species in the sighting";
                    break;
                case "behaviour":
                    group = "whatitdoes";
                    label = "Behaviour";
                    description = "Any behaviour of a species in the sighting";
                    break;
                case "food":
                    group = "whatitdoes";
                    label = "Food";
                    description = "The feeding chracteristics of the species in the sighting";
                    break;
                case "lifecycle":
                    group = "whatitdoes";
                    label = "Life Cycle";
                    description = "The life cycle stage or breeding charatcertic of the species in the sighting";
                    break;
                case "indigenouscommonnames":
                    group = "cultural";
                    label = "Indigenous Common Names";
                    description = "Any indigenous common names associated with the sighting";
                    break;
                case "indigenoususage":
                    group = "cultural";
                    label = "Usage in Indigenous Culture";
                    description = "Any special usage in indigenous cultures of the species in the sighting";
                    break;
                case "traditionalstories":
                    group = "cultural";
                    label = "Traditional Stories";
                    description = "Any traditional stories associated with the species in the sighting";
                    break;
                case "general":
                    group = "other";
                    label = "General Details";
                    description = "Any other general details";
                    break;
            }

            return new SightingNoteDescription(id, group, label, description, text);
        }

        public ISubContribution GetSubContribution(string type, string id)
        {
            return null;
        }

        #endregion
    }
}