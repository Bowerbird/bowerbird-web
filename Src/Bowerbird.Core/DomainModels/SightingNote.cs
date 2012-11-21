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
    public class SightingNote
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private IEnumerable<SightingNoteDescription> _descriptions;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore] 
        private IEnumerable<string> _tags;

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
            Identification identification,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            DateTime createdOn)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(tags, "tags");

            Id = id;
            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                identification,
                tags,
                descriptions);
        }

        #endregion

        #region Properties

        public int Id { get; private set; }
        
        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public Identification Identification { get; private set; }
        
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

        #endregion

        #region Methods

        private void InitMembers()
        {
            _descriptions = new List<SightingNoteDescription>();
            _tags = new List<string>();
        }

        protected void SetDetails(
            Identification identification, 
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions
            )
        {
            Identification = identification;
            Tags = tags;
            Descriptions = descriptions.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => MakeSightingNoteDescription(x.Key, x.Value));
        }

        public SightingNote UpdateDetails(
            User updatedByUser, 
            Identification identification, 
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(descriptions, "descriptions");

            SetDetails(
                identification,
                tags,
                descriptions);

            return this;
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

        #endregion
    }
}