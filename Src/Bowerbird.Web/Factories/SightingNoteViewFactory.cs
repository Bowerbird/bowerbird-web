using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class SightingNoteViewFactory : ISightingNoteViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public SightingNoteViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeNewSightingNote(string sightingId)
        {
            return new
            {
                Id = string.Empty,
                SightingId = sightingId,
                IsCustomIdentification = false,
                Tags = string.Empty,
                Descriptions = new object[] { }
            };
        }

        public dynamic Make(All_Contributions.Result result)
        {
            var sighting = result.Contribution as Sighting;

            var note = sighting.Notes.Single(x => x.Id.ToString() == result.ContributionSubId);

            return Make(note, result.User);
        }

        public dynamic Make(SightingNote sightingNote, User user)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sightingNote.Id;
            viewModel.CreatedOn = sightingNote.CreatedOn;
            viewModel.Identification = sightingNote.Identification;
            viewModel.Descriptions = sightingNote.Descriptions;
            viewModel.Tags = sightingNote.Tags;
            viewModel.User = _userViewFactory.Make(user);
            viewModel.TagCount = sightingNote.Tags.Count();
            viewModel.DescriptionCount = sightingNote.Descriptions.Count();

            return viewModel;
        }

        #endregion  
 
    }
}
