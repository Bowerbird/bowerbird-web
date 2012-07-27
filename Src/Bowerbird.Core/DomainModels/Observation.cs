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
    public class Observation : Sighting
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
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
            IEnumerable<Tuple<MediaResource, string, string>> addMedia
            ) 
            : base(
            createdByUser,
            createdOn,
            observedOn,
            latitude,
            longitude,
            anonymiseLocation,
            category,
            userProject,
            projects
            )
        {
            Check.RequireNotNull(addMedia, "addMedia");

            InitMembers();

            SetDetails(
                title,
                observedOn,
                latitude,
                longitude,
                address,
                isIdentificationRequired,
                anonymiseLocation,
                category);

            foreach (var media in addMedia)
            {
                AddMedia(media.Item1, media.Item2, media.Item3);
            }

            EnableEvents();

            FireEvent(new DomainModelCreatedEvent<Observation>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Title { get; private set; }

        public string Address { get; private set; }

        public bool IsIdentificationRequired { get; private set; }

        public IEnumerable<ObservationMedia> Media
        {
            get { return _observationMedia; }
            private set { _observationMedia = new List<ObservationMedia>(value); }
        }

        public ObservationMedia PrimaryMedia
        {
            get { return _observationMedia.First(); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _observationMedia = new List<ObservationMedia>();
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
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;

            base.SetDetails(
                observedOn,
                latitude,
                longitude,
                anonymiseLocation,
                category);
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

        public Observation AddMedia(MediaResource mediaResource, string description, string licence)
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            _observationMedia.Add(new ObservationMedia(mediaResource, description, licence));

            return this;
        }

        public Observation RemoveMedia(string mediaResourceId)
        {
            _observationMedia.RemoveAll(x => x.MediaResource.Id == mediaResourceId);

            return this;
        }

        #endregion
    }
}