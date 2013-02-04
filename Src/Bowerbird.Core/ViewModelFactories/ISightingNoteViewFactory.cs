using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface ISightingNoteViewFactory
    {
        object MakeCreateSightingNote(string sightingId);

        object MakeUpdateSightingNote(Sighting sighting, User user, int sightingNoteId);

        object Make(Sighting sighting, SightingNote sightingNote, User user, User authenticatedUser);
    }
}
