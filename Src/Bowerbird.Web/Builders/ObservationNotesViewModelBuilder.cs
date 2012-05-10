/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class ObservationNotesViewModelBuilder : IObservationNotesViewModelBuilder
    {
        #region Fields

        private readonly IObservationNoteViewFactory _observationNoteViewFactory;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationNotesViewModelBuilder(
            IObservationNoteViewFactory observationNoteViewFactory,
            IStreamItemFactory streamItemFactory,
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(observationNoteViewFactory, "observationNoteViewFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _observationNoteViewFactory = observationNoteViewFactory;
            _streamItemFactory = streamItemFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        public object BuildObservationNote(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _observationNoteViewFactory.Make(_documentSession.Load<ObservationNote>(idInput.Id));
        }

        public object BuildObservationNoteList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var observationNotes = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .Select(x => _observationNoteViewFactory.Make(x.ObservationNote))
                .ToArray();

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                ObservationNotes = observationNotes.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }
        
        public object BuildObservationNoteStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("ObservationNote") && x.GroupId == pagingInput.Id)
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "ObservationNote":
                    item = _observationNoteViewFactory.Make(groupContributionResult.ObservationNote);
                    description = groupContributionResult.ObservationNote.User.FirstName + " added an observation note";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                groups,
                "observationnote",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }

        #endregion
    }
}