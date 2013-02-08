using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class SightingNoteViewFactory : ISightingNoteViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public SightingNoteViewFactory(
            IUserViewFactory userViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeCreateSightingNote(string sightingId)
        {
            return new
            {
                SightingId = sightingId,
                Tags = string.Empty,
                Descriptions = new object[] { },
                NoteComments = string.Empty
            };
        }

        public object MakeUpdateSightingNote(Sighting sighting, User user, int sightingNoteId)
        {
            var sightingNote = sighting.Notes.Single(x => x.SequentialId == sightingNoteId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sightingNoteId;
            viewModel.SightingId = sighting.Id;
            viewModel.Descriptions = sightingNote.Descriptions.Select(x =>
                                                                      new
                                                                      {
                                                                          Key = x.Id,
                                                                          Value = x.Text,
                                                                          x.Description,
                                                                          x.Group,
                                                                          x.Label
                                                                      });
            viewModel.Tags = string.Join(", ", sightingNote.Tags);
            viewModel.NoteComments = sightingNote.Comments;

            return viewModel;
        }

        public object Make(Sighting sighting, SightingNote sightingNote, User user, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sightingNote.Id;
            viewModel.SightingId = sighting.Id;
            viewModel.CreatedOn = sightingNote.CreatedOn;
            viewModel.NoteComments = sightingNote.Comments;
            viewModel.Descriptions = sightingNote.Descriptions;
            viewModel.Tags = sightingNote.Tags;
            viewModel.User = _userViewFactory.Make(user, authenticatedUser); 
            viewModel.TagCount = sightingNote.Tags.Count();
            viewModel.DescriptionCount = sightingNote.Descriptions.Count(); 
            viewModel.AllTags = string.Join(", ", sightingNote.Tags);
            viewModel.CreatedOnDescription = sightingNote.CreatedOn.ToString("d MMMM yyyy");
            viewModel.TotalVoteScore = sightingNote.Votes.Sum(x => x.Score);

            // Current user-specific properties
            if (authenticatedUser != null)
            {
                var userId = authenticatedUser.Id;

                viewModel.UserVoteScore = sightingNote.Votes.Any(x => x.User.Id == userId) ? sightingNote.Votes.Single(x => x.User.Id == userId).Score : 0;
                viewModel.IsOwner = sightingNote.User.Id == authenticatedUser.Id;
            }

            return viewModel;
        }

        #endregion  
 
    }
}
