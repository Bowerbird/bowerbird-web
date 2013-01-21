/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using System.Collections.Generic;
using System;
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Sighting : DomainModel, IOwnable, IDiscussable, IVotable, IContribution
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<SightingGroup> _sightingGroups;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<SightingNote> _sightingNotes;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<IdentificationNew> _identifications;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Vote> _votes;

        #endregion

        #region Constructors

        protected Sighting() 
            : base() 
        {
            InitMembers();
        }

        protected Sighting(
            string key,
            User createdByUser,
            DateTime createdOn,
            DateTime observedOn,
            string latitude, 
            string longitude, 
            bool anonymiseLocation,
            string category,
            UserProject userProject,
            IEnumerable<Project> projects)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(userProject, "userProject");
            Check.RequireNotNull(projects, "projects");

            InitMembers();

            Key = key;
            User = createdByUser;
            CreatedOn = createdOn;

            _sightingGroups.Add(new SightingGroup(userProject, createdByUser, createdOn));

            SetSightingDetails(
                createdByUser,
                createdOn,
                observedOn,
                latitude, 
                longitude, 
                anonymiseLocation,
                category,
                projects);
        }

        #endregion

        #region Properties

        public string Key { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public DateTime ObservedOn { get; private set; }

        public string Latitude { get; private set; }
               
        public string Longitude { get; private set; }
               
        public bool AnonymiseLocation { get; private set; }
        
        public string Category { get; private set; }

        public IEnumerable<SightingGroup> Groups 
        {
            get { return _sightingGroups; }
            private set { _sightingGroups = new List<SightingGroup>(value); } 
        }

        public IEnumerable<SightingNote> Notes 
        { 
            get { return _sightingNotes; }
            private set { _sightingNotes = new List<SightingNote>(value); } 
        }

        public IEnumerable<IdentificationNew> Identifications
        {
            get { return _identifications; }
            private set { _identifications = new List<IdentificationNew>(value); }
        }

        public IEnumerable<Vote> Votes
        {
            get { return _votes; }
            private set { _votes = new List<Vote>(value); }
        }

        public Discussion Discussion { get; private set; }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return this.Groups.Select(x => x.Group.Id); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _sightingGroups = new List<SightingGroup>();
            _sightingNotes = new List<SightingNote>();
            _identifications = new List<IdentificationNew>();
            _votes = new List<Vote>();
            Discussion = new Discussion();
        }

        protected void SetSightingDetails(
            User user,
            DateTime updatedDateTime,
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            bool anonymiseLocation,
            string category,
            IEnumerable<Project> projects)
        {
            ObservedOn = observedOn;
            Latitude = latitude;
            Longitude = longitude;
            AnonymiseLocation = anonymiseLocation;
            Category = category;

            _sightingGroups.RemoveAll(x => x.Group.GroupType == "project");

            foreach (var project in projects)
            {
                _sightingGroups.Add(new SightingGroup(project, user, updatedDateTime));
            }
        }

        private SightingNote SetSightingNote(
            int id,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            string comments,
            DateTime createdOn,
            User createdByUser)
        {
            var sightingNote = new SightingNote(
                id,
                createdByUser,
                tags,
                descriptions,
                comments,
                createdOn);

            _sightingNotes.Add(sightingNote);

            return sightingNote;
        }

        private IdentificationNew SetIdentification(
            int id,
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
            IEnumerable<string> synonyms,
            DateTime createdOn,
            User createdByUser)
        {
            var identification = new IdentificationNew(
                id,
                createdByUser,
                createdOn,
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

            _identifications.Add(identification);

            return identification;
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

        public Sighting AddNote(
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            string comments,
            DateTime createdOn,
            User createdByUser)
        {
            Check.RequireNotNull(tags, "tags");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(createdByUser, "createdByUser");

            var maxId = _sightingNotes.Count > 0 ? _sightingNotes.Select(x => x.SequentialId).Max() : 0;

            SightingNote sightingNote = SetSightingNote(maxId + 1, tags, descriptions, comments, createdOn, createdByUser);

            ApplyEvent(new DomainModelCreatedEvent<SightingNote>(sightingNote, createdByUser, this));

            return this;
        }

        public Sighting UpdateNote(
            int id,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            string comments,
            DateTime updatedOn,
            User updatedByUser)
        {
            Check.RequireNotNull(tags, "tags");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SightingNote sightingNote = _sightingNotes.Single(x => x.SequentialId == id);

            sightingNote.UpdateDetails(
            updatedByUser,
            tags,
            descriptions,
            comments);

            ApplyEvent(new DomainModelUpdatedEvent<SightingNote>(sightingNote, updatedByUser, this));

            return this;
        }

        public Sighting RemoveNote(int sightingNoteId)
        {
            _sightingNotes.RemoveAll(x => x.SequentialId == sightingNoteId);

            return this;
        }

        public Sighting AddIdentification(
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
            IEnumerable<string> synonyms,            
            DateTime createdOn,
            User createdByUser)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(category, "category");
            Check.RequireNotNullOrWhitespace(kingdom, "kingdom"); // Can only check for kingdom as some Ids such as Funghi can be identified by kingdom alone
            Check.RequireNotNull(commonGroupNames, "commonGroupNames");
            Check.RequireNotNull(commonNames, "commonNames");
            Check.RequireNotNull(synonyms, "synonyms");
            Check.RequireNotNull(createdByUser, "createdByUser");

            var maxId = _identifications.Count > 0 ? _identifications.Select(x => x.SequentialId).Max() : 0;

            IdentificationNew identification = SetIdentification(
                maxId + 1,
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
                synonyms,
                createdOn,
                createdByUser);

            ApplyEvent(new DomainModelCreatedEvent<IdentificationNew>(identification, createdByUser, this));

            return this;
        }

        public Sighting UpdateIdentification(
            int id,
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
            IEnumerable<string> synonyms,            
            DateTime updatedOn,
            User updatedByUser)
        {
            Check.RequireNotNullOrWhitespace(category, "category");
            Check.RequireNotNullOrWhitespace(kingdom, "kingdom"); // Can only check for kingdom as some Ids such as Funghi can be identified by kingdom alone
            Check.RequireNotNull(commonGroupNames, "commonGroupNames");
            Check.RequireNotNull(commonNames, "commonNames");
            Check.RequireNotNull(synonyms, "synonyms");
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            IdentificationNew identification = _identifications.Single(x => x.SequentialId == id);

            identification.UpdateDetails(
                updatedByUser,
                updatedOn,
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

            ApplyEvent(new DomainModelUpdatedEvent<IdentificationNew>(identification, updatedByUser, this));

            return this;
        }

        public Sighting RemoveIdentification(int identificationId)
        {
            _identifications.RemoveAll(x => x.SequentialId == identificationId);

            return this;
        }

        public Vote UpdateVote(
            int score,
            DateTime createdOn,
            User createdByUser,
            string subContributionId = null)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            if (string.IsNullOrWhiteSpace(subContributionId))
            {
                _votes.RemoveAll(x => x.User.Id == createdByUser.Id);

                if (score != 0)
                {
                    var maxId = _votes.Count > 0 ? _votes.Select(x => x.SequentialId).Max() : 0;

                    Vote vote = SetVote(maxId + 1, score, createdOn, createdByUser);

                    ApplyEvent(new DomainModelCreatedEvent<Vote>(vote, createdByUser, this));

                    return vote;
                }
            }
            else
            {
                if (subContributionId.ToLower().StartsWith("notes/"))
                {
                    var vote = _sightingNotes.Single(x => x.Id == subContributionId).UpdateVote(score, createdOn, createdByUser);

                    if (vote != null)
                    {
                        ApplyEvent(new DomainModelCreatedEvent<Vote>(vote, createdByUser, this));
                    }
                }
                else if (subContributionId.ToLower().StartsWith("identifications/"))
                {
                    var vote = _identifications.Single(x => x.Id == subContributionId).UpdateVote(score, createdOn, createdByUser);

                    if (vote != null)
                    {
                        ApplyEvent(new DomainModelCreatedEvent<Vote>(vote, createdByUser, this));
                    }
                }
            }

            return null;
        }

        public Sighting AddToFavourites(Favourites favourites, User user, DateTime createdDateTime)
        {
            if (_sightingGroups.Any(x => x.Group.Id == favourites.Id))
            {
                // Already exists, remove it
                _sightingGroups.RemoveAll(x => x.Group.Id == favourites.Id);
            }
            else
            {
                // Doesn't exist, so add it
                var sightingGroup = new SightingGroup(favourites, user, createdDateTime);
                _sightingGroups.Add(sightingGroup);

                ApplyEvent(new DomainModelCreatedEvent<SightingGroup>(sightingGroup, user, this));
            }

            return this;
        }

        public ISubContribution GetSubContribution(string type, string id)
        {
            if (type == "identification")
            {
                return _identifications.SingleOrDefault(x => x.Id == id);
            }
            else if (type == "note")
            {
                return _sightingNotes.SingleOrDefault(x => x.Id == id);
            } 
            else if (type == "vote")
            {
                return _votes.SingleOrDefault(x => x.Id == id);
            }

            return null;
        }

        #endregion
    }
}