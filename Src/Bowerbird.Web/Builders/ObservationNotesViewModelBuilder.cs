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
using System;

namespace Bowerbird.Web.Builders
{
    public class ObservationNotesViewModelBuilder : IObservationNotesViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationNotesViewModelBuilder(
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        public object BuildObservationNote(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return MakeObservationNote(_documentSession.Load<ObservationNote>(idInput.Id));
        }

        public object BuildObservationNoteList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeObservationNote(x.ObservationNote))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
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
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            throw new NotImplementedException();
            //object item = null;
            //string description = null;
            //IEnumerable<string> groups = null;

            //switch (groupContributionResult.ContributionType)
            //{
            //    case "ObservationNote":
            //        item = _observationNoteViewFactory.Make(groupContributionResult.ObservationNote);
            //        description = groupContributionResult.ObservationNote.User.FirstName + " added an observation note";
            //        groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
            //        break;
            //}

            //return _streamItemFactory.Make(
            //    item,
            //    groups,
            //    "observationnote",
            //    groupContributionResult.GroupUser,
            //    groupContributionResult.GroupCreatedDateTime,
            //    description);
        }

        public object MakeObservationNote(ObservationNote observationNote)
        {
            throw new NotImplementedException();
            //return new
            //{
            //    observationNote.Id,
            //    ObservationId = observationNote.Observation.Id,
            //    observationNote.CreatedOn,
            //    observationNote.CommonName,
            //    observationNote.ScientificName,
            //    observationNote.Taxonomy,
            //    observationNote.Descriptions,
            //    observationNote.References,
            //    observationNote.Tags,
            //    CreatedBy = _userViewFactory.Make(observationNote.User.Id)
            //};
        }

        #endregion
    }
}