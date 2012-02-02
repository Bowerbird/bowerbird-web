using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Group : DomainModel, INamedDomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected Group()
        {
        }

        protected Group(
            string name)
            : this()
        {
            Check.RequireNotNullOrWhitespace(name, "name");

            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Website { get; private set; }

        #endregion

        #region Methods

        protected void SetDetails(string name, string description, string website)
        {
            Name = name;
            Description = description;
            Website = website;
        }

        #endregion      
      
    }
}
