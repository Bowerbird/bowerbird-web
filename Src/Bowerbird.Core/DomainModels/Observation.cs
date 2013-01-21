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
        }

        public Observation(
            string key,
            User createdByUser,
            string title,
            DateTime createdOn,
            DateTime observedOn,
            string latitude,
            string longitude,
            string address,
            bool anonymiseLocation,
            string category,
            UserProject userProject,
            IEnumerable<Project> projects,
            IEnumerable<Tuple<MediaResource, string, string, bool>> media
            ) 
            : base(
            key,
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
            InitMembers();

            SetObservationDetails(
                title,
                address,
                media);

            ApplyEvent(new SightingCreatedEvent(this, createdByUser, this, projects));
        }

        #endregion

        #region Properties

        public string Title { get; private set; }

        public string Address { get; private set; }

        public IEnumerable<ObservationMedia> Media
        {
            get { return _observationMedia; }
            private set { _observationMedia = new List<ObservationMedia>(value); }
        }

        public ObservationMedia PrimaryMedia
        {
            get { return _observationMedia.Single(x => x.IsPrimaryMedia); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _observationMedia = new List<ObservationMedia>();
        }

        private void SetObservationDetails(
            string title,
            string address,
            IEnumerable<Tuple<MediaResource, string, string, bool>> media)
        {
            Title = title;
            Address = address;

            _observationMedia.Clear();

            foreach (var mediaItem in media)
            {
                _observationMedia.Add(new ObservationMedia(mediaItem.Item1, mediaItem.Item2, mediaItem.Item3, mediaItem.Item4));
            }
        }

        public Observation UpdateDetails(
            User updatedByUser,
            DateTime updatedOn,
            string title,
            DateTime observedOn,
            string latitude,
            string longitude,
            string address,
            bool anonymiseLocation,
            string category,
            IEnumerable<Project> projects,
            IEnumerable<Tuple<MediaResource, string, string, bool>> media)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetSightingDetails(
                updatedByUser,
                updatedOn,
                observedOn,
                latitude,
                longitude,
                anonymiseLocation,
                category,
                projects);

            SetObservationDetails(
                title,
                address,
                media);

            ApplyEvent(new DomainModelUpdatedEvent<Observation>(this, updatedByUser, this));

            return this;
        }

        #endregion
    }
}