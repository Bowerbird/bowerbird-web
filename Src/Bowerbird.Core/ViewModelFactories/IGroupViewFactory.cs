using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface IGroupViewFactory
    {
        object Make(
            Group group, 
            User authenticatedUser, 
            bool fullDetails = false, 
            int sightingCount = 0, 
            int userCount = 0, 
            int postCount = 0, 
            IEnumerable<Observation> sampleObservations = null);
    }
}
