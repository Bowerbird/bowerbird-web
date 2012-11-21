using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Factories
{
    public interface ISightingNoteViewFactory : IFactory
    {
        object MakeCreateSightingNote(string sightingId);

        object MakeUpdateSightingNote(Sighting sighting, User user, int sightingNoteId);

        dynamic Make(All_Contributions.Result result);

        dynamic Make(SightingNote sightingNote, User user);
    }
}
