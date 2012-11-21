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
    public abstract class Sighting : DomainModel, IOwnable, IDiscussable
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<SightingGroup> _sightingGroups;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<SightingNote> _sightingNotes;

        #endregion

        #region Constructors

        protected Sighting() 
            : base() 
        {
            InitMembers();
        }

        protected Sighting(
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

            _sightingGroups.RemoveAll(x => x.Group.GroupType != "userproject");

            foreach (var project in projects)
            {
                _sightingGroups.Add(new SightingGroup(project, user, updatedDateTime));
            }
        }

        private SightingNote SetSightingNote(
            int id,
            Identification identification,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            DateTime createdOn,
            User createdByUser)
        {
            var sightingNote = new SightingNote(
                id,
                createdByUser,
                identification,
                tags,
                descriptions,
                createdOn);

            _sightingNotes.Add(sightingNote);

            return sightingNote;
        }

        public Sighting AddNote(
            Identification identification,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            DateTime createdOn,
            User createdByUser)
        {
            Check.RequireNotNull(tags, "tags");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(createdByUser, "createdByUser");

            var maxId = _sightingNotes.Count > 0 ? _sightingNotes.Select(x => x.Id).Max() : 0;

            SightingNote sightingNote = SetSightingNote(maxId + 1, identification, tags, descriptions, createdOn, createdByUser);

            ApplyEvent(new DomainModelCreatedEvent<SightingNote>(sightingNote, createdByUser, this));

            return this;
        }

        public Sighting UpdateNote(
            int id,
            Identification identification,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            DateTime updatedOn,
            User updatedByUser)
        {
            Check.RequireNotNull(tags, "tags");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SightingNote sightingNote = _sightingNotes.Single(x => x.Id == id);

            sightingNote.UpdateDetails(
            updatedByUser,
            identification,
            tags,
            descriptions);

            ApplyEvent(new DomainModelUpdatedEvent<SightingNote>(sightingNote, updatedByUser, this));

            return this;
        }

        public Sighting RemoveNote(int sightingNoteId)
        {
            _sightingNotes.RemoveAll(x => x.Id == sightingNoteId);

            return this;
        }

        //public Sighting AddGroup(Group group, User createdByUser, DateTime createdDateTime)
        //{
        //    Check.RequireNotNull(group, "group");
        //    Check.RequireNotNull(createdByUser, "createdByUser");

        //    var sightingGroup = SetSightingGroup(group, createdByUser, createdDateTime);

        //    if(sightingGroup != null)
        //    {
        //        ApplyEvent(new SightingGroupCreatedEvent(sightingGroup, createdByUser, this, group));
        //    }

        //    return this;
        //}

        //public Sighting RemoveGroup(string groupId)
        //{
        //    _sightingGroups.RemoveAll(x => x.Group.Id == groupId);

        //    return this;
        //}

        #endregion
    }
}