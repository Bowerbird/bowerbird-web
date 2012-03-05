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

namespace Bowerbird.Core.DomainModels
{
    public class Observation : Contribution
    {
        #region Members

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
            IDictionary<MediaResource, string> observationMediaItems)
            : base(
            createdByUser,
            createdOn)
        {
            Check.RequireNotNull(observationMediaItems, "observationMediaItems");

            InitMembers();

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                observationCategory,
                observationMediaItems);

            var eventMessage = string.Format(
                ActivityMessage.CreatedAnObservation,
                createdByUser.GetName(),
                title
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<Observation>(this, createdByUser, eventMessage));
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

        public IEnumerable<Comment> Comments
        {
            get { return _comments; }

            private set { _comments = value as List<Comment>; }
        }

        public IEnumerable<ObservationMedia> ObservationMedia { get { return _observationMedia; } }

        #endregion

        #region Methods

        public override string ContributionType()
        {
            return "Observation";
        }

        public override string ContributionTitle()
        {
            return Title;
        }

        private void InitMembers()
        {
            _observationMedia = new List<ObservationMedia>();

            _comments = new List<Comment>();
        }

        private void SetDetails(string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired, 
            string observationCategory, 
            IDictionary<MediaResource, string> observationMediaItems)
        {
            Title = title;
            ObservedOn = observedOn;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;
            ObservationCategory = observationCategory;

            _observationMedia.Clear();

            foreach (var observationMediaItem in observationMediaItems)
            {
                var observationMedia = new ObservationMedia(observationMediaItem.Key, observationMediaItem.Value);

                _observationMedia.Add(observationMedia);
            }
        }

        public Observation UpdateDetails(User updatedByUser, 
            string title, 
            DateTime observedOn, 
            string latitude, 
            string longitude, 
            string address, 
            bool isIdentificationRequired, 
            string observationCategory, 
            IDictionary<MediaResource, string> observationMediaItems)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(observationMediaItems, "observationMediaItems");

            SetDetails(
                title,
                observedOn,
                latitude,
                longitude,
                address,
                isIdentificationRequired,
                observationCategory,
                observationMediaItems);

            var eventMessage = string.Format(
                ActivityMessage.UpdatedAnObservation,
                updatedByUser.GetName(),
                title
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<Observation>(this, updatedByUser, eventMessage));

            return this;
        }

        #endregion
    }
}