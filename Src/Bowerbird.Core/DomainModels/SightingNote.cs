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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class SightingNote
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

        protected SightingNote() 
            : base() 
        {
            InitMembers();
        }

        public SightingNote(
            User createdByUser,
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
            Check.RequireNotNull(descriptions, "descriptions");
            Check.RequireNotNull(references, "references");
            Check.RequireNotNull(tags, "tags");

            InitMembers();

            User = createdByUser;
            CreatedOn = createdOn;

            SetDetails(
                commonName,
                scientificName,
                taxonomy,
                tags,
                descriptions,
                references);
        }

        #endregion

        #region Properties

        public string Id { get; private set; }
        
        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }
        
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

        public SightingNote UpdateDetails(
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

            return this;
        }

        #endregion
    }
}