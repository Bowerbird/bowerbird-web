﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class ObservationNote : DomainModel, IOwnable, IContribution
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private Dictionary<string, string> _descriptions;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private Dictionary<string, string> _references;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore] 
        private IEnumerable<string> _tags;

        #endregion

        #region Constructors

        protected ObservationNote() 
            : base() 
        {
            InitMembers();

            EnableEvents();
        }

        public ObservationNote(
            User createdByUser,
            Observation observation,
            string commonName, 
            string scientificName, 
            string taxonomy,
            IEnumerable<string> tags,
            IDictionary<string, string> descriptions,
            IDictionary<string, string> references,
            DateTime createdOn)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(observation, "observation");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(references, "references");
            Check.RequireNotNull(tags, "tags");

            InitMembers();

            User = createdByUser;
            CreatedOn = createdOn;
            Observation = observation;

            SetDetails(
                commonName,
                scientificName,
                taxonomy,
                tags,
                descriptions,
                references);

            EnableEvents();
            FireEvent(new DomainModelCreatedEvent<ObservationNote>(this, createdByUser, this));
        }

        #endregion

        #region Properties
        
        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }
        
        public DenormalisedObservationReference Observation { get; private set; }
        
        public string ScientificName { get; private set; }
        
        public string CommonName { get; private set; }
        
        public string Taxonomy { get; private set; }
        
        public IEnumerable<string> Tags
        {
            get { return _tags; }
            private set { _tags = new List<string>(value); }
        }

        public IDictionary<string, string> Descriptions 
        {
            get { return _descriptions; }
            private set { _descriptions = new Dictionary<string,string>(value); }
        }

        public IDictionary<string, string> References
        {
            get { return _references; }
            private set { _references = new Dictionary<string, string>(value); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return this.Observation.Groups; }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _descriptions = new Dictionary<string, string>();
            _references = new Dictionary<string, string>();
            _tags = new List<string>();
        }

        protected void SetDetails(
            string commonName, 
            string scientificName, 
            string taxonomy, 
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions, 
            IDictionary<string, string> references
            )
        {
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(references, "references");

            CommonName = commonName;
            ScientificName = scientificName;
            Taxonomy = taxonomy;
            Tags = tags;
            Descriptions = descriptions.ToDictionary(x => x.Key, x => x.Value);
            References = references.ToDictionary(x => x.Key, x => x.Value);
        }

        public ObservationNote UpdateDetails(
            User updatedByUser, 
            string commonName, 
            string scientificName, 
            string taxonomy, 
            IEnumerable<string> tags, 
            IDictionary<string, string> descriptions, 
            IDictionary<string, string> references)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(references, "references");

            SetDetails(
                commonName,
                scientificName,
                taxonomy,
                tags,
                descriptions,
                references);

            FireEvent(new DomainModelUpdatedEvent<ObservationNote>(this, updatedByUser, this));

            return this;
        }

        #endregion
    }
}