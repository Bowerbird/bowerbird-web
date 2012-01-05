using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.Entities
{
    public class ObservationNote : Entity
    {
        #region Fields

        #endregion

        #region Constructors

        protected ObservationNote() 
            : base() 
        {
            InitMembers();
        }

        public ObservationNote(
            User createdByUser,
            Observation observation,
            string commonName, 
            string scientificName, 
            string taxonomy,
            string tags,
            IDictionary<string, string> descriptions,
            IDictionary<string, string> references,
            string notes)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(observation, "observation");
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(references, "references");

            User = createdByUser;
            Observation = observation;
            SubmittedOn = DateTime.Now;

            SetDetails(
                commonName,
                scientificName,
                taxonomy,
                tags,
                descriptions,
                references,
                notes);

            EventProcessor.Raise(new EntityCreatedEvent<ObservationNote>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DenormalisedObservationReference Observation { get; private set; }

        public DateTime SubmittedOn { get; private set; }

        public string ScientificName { get; private set; }

        public string CommonName { get; private set; }

        public string Taxonomy { get; private set; }

        public string Tags { get; private set; }

        public Dictionary<string, string> Descriptions { get; private set; }

        public Dictionary<string, string> References { get; private set; }

        public string Notes { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Descriptions = new Dictionary<string, string>();
            References = new Dictionary<string, string>();
        }

        protected void SetDetails(string commonName, string scientificName, string taxonomy, string tags, IDictionary<string, string> descriptions, IDictionary<string, string> references, string notes)
        {
            CommonName = commonName;
            ScientificName = scientificName;
            Taxonomy = taxonomy;
            Tags = tags;
            Notes = notes;
            Descriptions = descriptions.ToDictionary(x => x.Key, x => x.Value);
            References = references.ToDictionary(x => x.Key, x => x.Value);
        }

        public ObservationNote UpdateDetails(User updatedByUser, string commonName, string scientificName, string taxonomy, string tags, IDictionary<string, string> descriptions, IDictionary<string, string> references, string notes)
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
                references,
                notes);

            EventProcessor.Raise(new EntityUpdatedEvent<ObservationNote>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}