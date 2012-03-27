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

        private List<ObservationMedia> _media;

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
            string observationCategory)
            : base(
            createdByUser,
            createdOn)
        {
            InitMembers();

            SetDetails(
                title, 
                observedOn,
                latitude, 
                longitude, 
                address,
                isIdentificationRequired,
                observationCategory);

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

        public IEnumerable<Comment> Comments
        {
            get { return _comments; }

            private set { _comments = value as List<Comment>; }
        }

        public IEnumerable<ObservationMedia> Media { get { return _media; } }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _media = new List<ObservationMedia>();

            _comments = new List<Comment>();
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

        public Observation AddMedia(MediaResource mediaResource, string description, string licence)
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            int id = 0;
            if (_media.Count > 0)
            {
                id = _media.Select(x => Convert.ToInt32(x.Id)).Max();
            }
            id++;

            _media.Add(new ObservationMedia(id.ToString(), mediaResource, description, licence));

            return this;
        }

        public Observation RemoveMedia(string mediaResourceId)
        {
            if (_media.Any(x => x.MediaResource.Id == mediaResourceId))
            {
                _media.RemoveAll(x => x.MediaResource.Id == mediaResourceId);
            }

            return this;
        }

        #endregion

    }
}