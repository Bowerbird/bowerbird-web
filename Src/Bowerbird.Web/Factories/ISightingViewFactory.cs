using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface ISightingViewFactory : IFactory
    {
        object MakeObservation();

        object Make(All_Contributions.Result result);

        object Make(Observation observation, User user);
    }
}
