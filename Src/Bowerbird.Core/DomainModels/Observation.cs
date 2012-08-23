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
            IEnumerable<Project> projects
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
            InitMembers();

            SetObservationDetails(
                title,
                address,
                isIdentificationRequired);

            ApplyEvent(new SightingCreatedEvent(this, createdByUser, this, projects));
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
            bool isIdentificationRequired)
        {
            Title = title;
            Address = address;
            IsIdentificationRequired = isIdentificationRequired;
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

            SetSightingDetails(
                observedOn,
                latitude,
                longitude,
                anonymiseLocation,
                category);

            SetObservationDetails(
                title,
                address,
                isIdentificationRequired);

            ApplyEvent(new DomainModelUpdatedEvent<Observation>(this, updatedByUser, this));

            return this;
        }

        public Observation AddMedia(MediaResource mediaResource, string description, string licence, bool isPrimaryMedia)
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            _observationMedia.Add(new ObservationMedia(mediaResource, description, licence, isPrimaryMedia));

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