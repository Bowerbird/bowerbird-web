using System;
using Bowerbird.Core.DomainModels;
namespace Bowerbird.Web.Factories
{
    public interface IObservationViewFactory : IFactory
    {
        object Make();

        object Make(Observation observation);
    }
}
