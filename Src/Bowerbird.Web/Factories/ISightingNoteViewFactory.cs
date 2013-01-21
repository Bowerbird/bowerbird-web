using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Factories
{
    public interface ISightingNoteViewFactory : IFactory
    {
        object MakeCreateIdentification(string sightingId);

        object MakeCreateSightingNote(string sightingId);

        object MakeUpdateIdentification(Sighting sighting, User user, int identificationId);

        object MakeUpdateSightingNote(Sighting sighting, User user, int sightingNoteId);

        dynamic Make(Sighting sighting, SightingNote sightingNote, User user, User authenticatedUser);

        dynamic Make(Sighting sighting, IdentificationNew identification, User user, User authenticatedUser);
    }
}
