/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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

namespace Bowerbird.Core.DomainModels
{
    public class Observation : Contribution
    {

        #region Members

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

            EventProcessor.Raise(new DomainModelUpdatedEvent<Observation>(this, updatedByUser));

            return this;
        }

        #endregion
      
    }
}