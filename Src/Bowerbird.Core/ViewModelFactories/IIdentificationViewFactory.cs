using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface IIdentificationViewFactory
    {
        object MakeCreateIdentification(string sightingId);

        object MakeUpdateIdentification(Sighting sighting, User user, int identificationId);

        object Make(Sighting sighting, IdentificationNew identification, User user, User authenticatedUser);
    }
}