/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using System.Collections.Generic;
using System;
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Observation : DomainModel, IOwnable, IContribution
    {

        #region Members

        [JsonIgnore]
        private List<ObservationGroup> _observationGroups;
        [JsonIgnore]
        private List<DenormalisedObservationNoteReference> _observationNotes;
        [JsonIgnore]
        private List<ObservationMedia> _observationMedia;

        #endregion

        #region Constructors

        protected Observation() 
            : base() 
        {
            InitMembers();
        }

        public Observation(
            User createdByUser,
            string title, 
            DateTime createdOn,
            DateTime observedOn,
            string latitude, 
            string longitude, 
            string address,
            bool isIdentificationRequired,
            string observationCategory,
            UserProject userProject,
            IEnumerable<Project> projects,
            IEnumerable<Tuple<MediaResource, string, string>> addMedia)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(userProject, "userProject");
            Check.RequireNotNull(projects, "projects");
            Check.RequireNotNull(addMedia, "addMedia");

            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                observationCategory);

            AddGroup(userProject, createdByUser, createdOn);

            foreach (var project in projects)
            {
                AddGroup(project, createdByUser, createdOn);
            }

            foreach (var media in addMedia)
            {
                AddMedia(media.Item1, media.Item2, media.Item3);
            }

            CanFireCreatedEvent = true;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public string Title { get; private set; }

        public DateTime ObservedOn { get; private set; }

        public string Latitude { get; private set; }
               
        public string Longitude { get; private set; }
               
        public string Address { get; private set; }
               
        public bool IsIdentificationRequired { get; private set; }
        
        public string ObservationCategory { get; private set; }

        public IEnumerable<ObservationMedia> Media 
        { 
            get { return _observationMedia; }
            private set { _observationMedia = new List<ObservationMedia>(value); } 
        }

        public IEnumerable<ObservationGroup> Groups 
        { 
            get { return _observationGroups; }
            private set { _observationGroups = new List<ObservationGroup>(value); } 
        }

        public IEnumerable<DenormalisedObservationNoteReference> Notes 
        { 
            get { return _observationNotes; }
            private set { _observationNotes = new List<DenormalisedObservationNoteReference>(value); } 
        }

        public CommentsComponent Discussion { get; private set; }

        [JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return this.Groups.Select(x => x.GroupId); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Discussion = new CommentsComponent();
            _observationMedia = new List<ObservationMedia>();
            _observationGroups = new List<ObservationGroup>();
            _observationNotes = new List<DenormalisedObservationNoteReference>();
        }

        protected override void FireCreateEvent()
        {
            EventProcessor.Raise(new DomainModelCreatedEvent<Observation>(this, User.Id));
            base.FireCreateEvent();
        }

        private void SetDetails(string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired, 
            string observationCategory)
        {
            Title = title;
            ObservedOn = observedOn;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;
            ObservationCategory = observationCategory;
        }

        public Observation UpdateDetails(User updatedByUser, 
            string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired, 
            string observationCategory)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                title,
                observedOn,
                latitude,
                longitude,
                address,
                isIdentificationRequired,
                observationCategory);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Observation>(this, updatedByUser));

            return this;
        }

        public Observation AddNote(ObservationNote observationNote)
        {
            Check.RequireNotNull(observationNote, "observationNote");

            if (_observationNotes.All(x => x.Id != observationNote.Id))
            {
                DenormalisedObservationNoteReference denormalisedObservationNoteReference = observationNote;
                _observationNotes.Add(denormalisedObservationNoteReference);
            }

            return this;
        }

        public Observation RemoveNote(string observationNoteId)
        {
            _observationNotes.RemoveAll(x => x.Id == observationNoteId);

            return this;
        }

        public Observation AddGroup(Group group, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            if (_observationGroups.All(x => x.GroupId != group.Id))
            {
                var observationGroup = new ObservationGroup(group, createdByUser, createdDateTime);

                _observationGroups.Add(observationGroup);

                if (CanFireCreatedEvent)
                {
                    EventProcessor.Raise(new DomainModelCreatedEvent<ObservationGroup>(observationGroup, createdByUser.Id));
                }
            }

            return this;
        }

        public Observation RemoveGroup(string groupId)
        {
            if (_observationGroups.Any(x => x.GroupId == groupId))
            {
                _observationGroups.RemoveAll(x => x.GroupId == groupId);
            }

            return this;
        }

        public Observation AddMedia(MediaResource mediaResource, string description, string licence)
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            int id = 0;
            if (_observationMedia.Count > 0)
            {
                id = _observationMedia.Select(x => Convert.ToInt32(x.Id)).Max();
            }
            id++;

            _observationMedia.Add(new ObservationMedia(id.ToString(), mediaResource, description, licence));

            return this;
        }

        public Observation RemoveMedia(string mediaResourceId)
        {
            if (_observationMedia.Any(x => x.MediaResource.Id == mediaResourceId))
            {
                _observationMedia.RemoveAll(x => x.MediaResource.Id == mediaResourceId);
            }

            return this;
        }

        public Observation AddComment(string message, User createdByUser, DateTime createdDateTime)
        {
            Discussion.AddComment(message, createdByUser, createdDateTime);

            return this;
        }

        public Observation RemoveComment(string commentId)
        {
            Discussion.RemoveComment(commentId);

            return this;
        }

        public Observation UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Discussion.UpdateComment(commentId, message, modifiedByUser, modifiedDateTime);

            return this;
        }

        #endregion

    }
}