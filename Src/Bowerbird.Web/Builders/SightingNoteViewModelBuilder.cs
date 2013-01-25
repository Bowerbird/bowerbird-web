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
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public SightingNoteViewModelBuilder(
            IDocumentSession documentSession,
            ISightingNoteViewFactory sightingNoteViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _sightingNoteViewFactory = sightingNoteViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Methods

        public object BuildCreateIdentification(string sightingId)
        {
            return _sightingNoteViewFactory.MakeCreateIdentification(sightingId);
        }

        public object BuildCreateSightingNote(string sightingId)
        {
            return _sightingNoteViewFactory.MakeCreateSightingNote(sightingId);
        }

        public object BuildUpdateIdentification(string sightingId, int identificationId)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == sightingId && x.SubContributionId == identificationId.ToString())
                .First();

            return _sightingNoteViewFactory.MakeUpdateIdentification(result.Observation, result.User, identificationId);
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

        public dynamic BuildSightingNote(string sightingId, int sightingNoteId)
        {
            var results = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == sightingId && (x.ParentContributionType == "observation" || x.ParentContributionType == "record"))
                .ToList();

            var result = results.Single(x => x.ParentContributionType == "observation" || x.ParentContributionType == "record");

            var sighting = result.ParentContribution as Sighting;
            var authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            dynamic sightingNote = _sightingNoteViewFactory.Make(sighting, sighting.Notes.Single(x => x.SequentialId == sightingNoteId), result.User, authenticatedUser);

            return sightingNote;
        }

        #endregion
    }
}