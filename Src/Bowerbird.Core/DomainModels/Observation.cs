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

namespace Bowerbird.Core.DomainModels
{
    public class Observation : Contribution
    {
        #region Members

        private List<Comment> _comments;

        private List<MediaResource> _mediaResources;

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
            IEnumerable<MediaResource> mediaResources)
            : base(
            createdByUser,
            createdOn)
        {
            Check.RequireNotNull(mediaResources, "mediaResources");

            Id = "observations/";

            InitMembers();

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                observationCategory,
                mediaResources);

            EventProcessor.Raise(new DomainModelCreatedEvent<Observation>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Title { get; private set; }

        public DateTime ObservedOn { get; private set; }

        public string Latitude { get; private set; }
               
        public string Longitude { get; private set; }
               
        public string Address { get; private set; }
               
        public bool IsIdentificationRequired { get; private set; }
        
        public string ObservationCategory { get; private set; }

        public IEnumerable<Comment> Comments { get { return _comments; } }

        public IEnumerable<MediaResource> MediaResources { get { return _mediaResources; } }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _mediaResources = new List<MediaResource>();

            _comments = new List<Comment>();
        }

        private void SetDetails(string title, DateTime observedOn, string latitude, string longitude, string address, bool isIdentificationRequired, string observationCategory, IEnumerable<MediaResource> mediaResources)
        {
            Title = title;
            ObservedOn = observedOn;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;
            ObservationCategory = observationCategory;
            _mediaResources = mediaResources.ToList();
        }

        public virtual Observation UpdateDetails(User updatedByUser, string title, DateTime observedOn, string latitude, string longitude, string address, bool isIdentificationRequired, string observationCategory, IEnumerable<MediaResource> mediaResources)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(mediaResources, "mediaResources");

            SetDetails(
                title,
                observedOn,
                latitude,
                longitude,
                address,
                isIdentificationRequired,
                observationCategory,
                mediaResources);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Observation>(this, updatedByUser));

            return this;
        }

        public void AddComment(Comment comment, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(comment, "comment");
            Check.RequireNotNull(createdByUser, "createdByUser");

            _comments.Add(comment);

            EventProcessor.Raise(new DomainModelCreatedEvent<Comment>(comment, createdByUser));
        }

        public void RemoveComment(string commentId)
        {
            if (_comments.Any(x => x.Id == commentId))
            {
                _comments.RemoveAll(x => x.Id == commentId);
            }
        }

        #endregion
    }
}