using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface IUserViewFactory
    {
        object Make(User result, User authenticatedUser, bool fullDetails = false, int? sightingCount = 0, IEnumerable<Observation> sampleObservations = null, int? followerCount = 0);
    }
}