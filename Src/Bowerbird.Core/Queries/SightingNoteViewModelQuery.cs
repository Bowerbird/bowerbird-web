/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.ViewModelFactories;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.Queries
{
    public class SightingNoteViewModelQuery : ISightingNoteViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;

        #endregion

        #region Constructors

        public SightingNoteViewModelQuery(
            IDocumentSession documentSession,
            ISightingNoteViewFactory sightingNoteViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");

            _documentSession = documentSession;
            _sightingNoteViewFactory = sightingNoteViewFactory;
        }

        #endregion

        #region Methods

        public object BuildCreateSightingNote(string sightingId)
        {
            return _sightingNoteViewFactory.MakeCreateSightingNote(sightingId);
        }

        public object BuildUpdateSightingNote(string sightingId, int sightingNoteId)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == sightingId && x.SubContributionId == sightingNoteId.ToString())
                .First();

            return _sightingNoteViewFactory.MakeUpdateSightingNote(result.Observation, result.User, sightingNoteId);
        }

        #endregion
    }
}