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
    public class SightingNoteViewModelBuilder : ISightingNoteViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;

        #endregion

        #region Constructors

        public SightingNoteViewModelBuilder(
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
                .Where(x => x.ContributionId == sightingId && x.ContributionSubId == sightingNoteId.ToString())
                .First();

            return _sightingNoteViewFactory.MakeUpdateSightingNote(result.Observation, result.User, sightingNoteId);
        }

        public dynamic BuildSightingNote(string sightingId, int sightingNoteId)
        {
            var results = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == sightingId && (x.ContributionType == "observation" || x.ContributionType == "record"))
                .ToList();

            var result = results.Single(x => x.ContributionType == "observation" || x.ContributionType == "record");

            dynamic sightingNote = _sightingNoteViewFactory.Make((result.Contribution as Sighting).Notes.Single(x => x.Id == sightingNoteId), result.User);

            return sightingNote;
        }

        #endregion
    }
}