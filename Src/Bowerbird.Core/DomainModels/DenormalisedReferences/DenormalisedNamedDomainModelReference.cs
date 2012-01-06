using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public interface INamedDomainModel
    {
        string Id { get; }

        string Name { get; }
    }

    public class DenormalisedNamedDomainModelReference<T> where T : INamedDomainModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Name { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedNamedDomainModelReference<T>(T namedDomainModel)
        {
            Check.RequireNotNull(namedDomainModel, "namedDomainModel");

            return new DenormalisedNamedDomainModelReference<T>
            {
                Id = namedDomainModel.Id, 
                Name = namedDomainModel.Name
            };
        }

        #endregion

    }
}
  