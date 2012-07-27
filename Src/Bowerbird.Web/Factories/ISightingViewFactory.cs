using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface ISightingViewFactory : IFactory
    {
        object MakeNewObservation(string projectId = null);

        object MakeNewRecord(string projectId = null);

        object Make(All_Contributions.Result result);

        object Make(Sighting sighting, User user);
    }
}
