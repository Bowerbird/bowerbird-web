using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Factories
{
    public interface ISightingNoteViewFactory : IFactory
    {
        object MakeNewSightingNote(string sightingId);

        dynamic Make(All_Contributions.Result result);

        dynamic Make(SightingNote sightingNote, User user);
    }
}
