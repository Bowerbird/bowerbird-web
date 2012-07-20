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
using System.Collections.Generic;
using System;
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Observation : DomainModel, IOwnable, IContribution, IDiscussed
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

            EnableEvents();
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
            bool anonymiseLocation,
            string category,
            UserProject userProject,
            IEnumerable<Project> projects,
            IEnumerable<Tuple<MediaResource, string, string>> addMedia)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(userProject, "userProject");
            Check.RequireNotNull(projects, "projects");
            Check.RequireNotNull(addMedia, "addMedia");

            InitMembers();

            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                anonymiseLocation,
                category);

            AddGroup(userProject, createdByUser, createdOn);

            foreach (var project in projects)
            {
                AddGroup(project, createdByUser, createdOn);
            }

            foreach (var media in addMedia)
            {
                AddMedia(media.Item1, media.Item2, media.Item3);
            }

            EnableEvents();

            FireEvent(new DomainModelCreatedEvent<Observation>(this, createdByUser, this));
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

        public bool AnonymiseLocation { get; private set; }
        
        public string Category { get; private set; }

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
            get { return this.Groups.Select(x => x.Group.Id); }
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

        private void SetDetails(string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired,
            bool anonymiseLocation,
            string category)
        {
            Title = title;
            ObservedOn = observedOn;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;
            AnonymiseLocation = anonymiseLocation;
            Category = category;
        }

        public Observation UpdateDetails(User updatedByUser, 
            string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired,
            bool anonymiseLocation,
            string category)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                title,
                observedOn,
                latitude,
                longitude,
                address,
                isIdentificationRequired,
                anonymiseLocation,
                category);

            FireEvent(new DomainModelUpdatedEvent<Observation>(this, updatedByUser, this));

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

            if (_observationGroups.All(x => x.Group.Id != group.Id))
            {
                var observationGroup = new ObservationGroup(group, createdByUser, createdDateTime);

                _observationGroups.Add(observationGroup);

                FireEvent(new DomainModelCreatedEvent<ObservationGroup>(observationGroup, createdByUser, this));
            }

            return this;
        }

        public Observation RemoveGroup(string groupId)
        {
            if (_observationGroups.Any(x => x.Group.Id == groupId))
            {
                _observationGroups.RemoveAll(x => x.Group.Id == groupId);
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

        IContribution IDiscussed.AddComment(string message, User createdByUser, DateTime createdDateTime, string contributionId)
        {
            var comment = Discussion.AddComment(message, createdByUser, createdDateTime, contributionId);

            FireEvent(new DomainModelCreatedEvent<Comment>(comment, createdByUser, this));

            return this;
        }

        IContribution IDiscussed.AddThreadedComment(string message, User createdByUser, DateTime createdDateTime, string commentToRespondTo, string contributionId)
        {
            var comment = Discussion.AddThreadedComment(message, createdByUser, createdDateTime, commentToRespondTo, contributionId);

            FireEvent(new DomainModelCreatedEvent<Comment>(comment, createdByUser, this));
            
            return this;
        }

        IContribution IDiscussed.RemoveComment(string commentId)
        {
            Discussion.RemoveComment(commentId);

            return this;
        }

        IContribution IDiscussed.UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Discussion.UpdateComment(commentId, message, modifiedByUser, modifiedDateTime);

            return this;
        }

        public ObservationMedia GetPrimaryMedia()
        {
            return Media.First();
        }

        #endregion
    }
}