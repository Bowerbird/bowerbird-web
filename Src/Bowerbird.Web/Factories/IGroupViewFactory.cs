using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface IGroupViewFactory : IFactory
    {
        dynamic Make(Group group, User authenticatedUser, bool fullDetails = false, int sightingCount = 0, int userCount = 0, int postCount = 0, IEnumerable<Observation> sampleObservations = null);
    }
}
