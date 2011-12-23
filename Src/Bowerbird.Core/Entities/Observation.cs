using Bowerbird.Core.DesignByContract;
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Entities.MediaResources;
using System;
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Observation : Entity
    {

        #region Fields

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
            DateTime observedOn,
            string latitude, 
            string longitude, 
            string address,
            bool isIdentificationRequired,
            string observationCategory,
            IEnumerable<MediaResource> mediaResources) 
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(mediaResources, "mediaResources");

            User = createdByUser;
            SubmittedOn = DateTime.Now;

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                observationCategory,
                mediaResources);

            EventProcessor.Raise(new EntityCreatedEvent<Observation>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public string Title { get; private set; }

        public DateTime ObservedOn { get; private set; }

        public DateTime SubmittedOn { get; private set; }
               
        public string Latitude { get; private set; }
               
        public string Longitude { get; private set; }
               
        public string Address { get; private set; }
               
        public bool IsIdentificationRequired { get; private set; }
        
        public string ObservationCategory { get; private set; }

        public List<MediaResource> MediaResources { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            MediaResources = new List<MediaResource>();
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
            MediaResources = mediaResources.ToList();
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

            EventProcessor.Raise(new EntityUpdatedEvent<Observation>(this, updatedByUser));

            return this;
        }

        #endregion
      
    }
}
