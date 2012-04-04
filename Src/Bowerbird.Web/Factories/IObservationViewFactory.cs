using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels.Shared;
namespace Bowerbird.Web.Factories
{
    public interface IObservationViewFactory : IFactory
    {
        ObservationView Make(Observation observation);
    }
}
