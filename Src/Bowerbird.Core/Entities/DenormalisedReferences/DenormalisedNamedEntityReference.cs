using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Entities.DenormalisedReferences
{
    public interface INamedEntity
    {
        string Id { get; }

        string Name { get; }
    }

    public class DenormalisedNamedEntityReference<T> where T : INamedEntity
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

        public static implicit operator DenormalisedNamedEntityReference<T>(T namedEntity)
        {
            return new DenormalisedNamedEntityReference<T>
            {
                Id = namedEntity.Id,
                Name = namedEntity.Name
            };
        }

        #endregion

    }
}
