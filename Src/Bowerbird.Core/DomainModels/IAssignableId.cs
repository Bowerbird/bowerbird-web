using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.DomainModels
{
    public interface IAssignableId
    {
        /// <summary>
        ///     Enables developer to set the assigned Id of an object.  This is not part of 
        ///     <see cref = "DomainModel" /> since most entities do not have assigned 
        ///     Ids and since business rules will certainly vary as to what constitutes a valid,
        ///     assigned Id for one object but not for another.
        /// </summary>
        void SetIdTo(string prefix, string assignedId);
    }
}
