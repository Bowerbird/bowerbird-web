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

        public object MakeCreateIdentification(string sightingId)
        {
            return new
            {
                SightingId = sightingId,
                IsCustomIdentification = false,
                Taxonomy = string.Empty
            };
        }

        public object MakeCreateSightingNote(string sightingId)
        {
            return new
            {
                SightingId = sightingId,
                IsCustomIdentification = false,
                Identification = (object)null,
                Tags = string.Empty,
                Descriptions = new object[] { },
                Comments = string.Empty,
                Taxonomy = string.Empty
            };
        }

        public object MakeUpdateIdentification(Sighting sighting, User user, int identificationId)
        {
            var identification = sighting.Identifications.Single(x => x.SequentialId == identificationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = identificationId;
            viewModel.SightingId = sighting.Id;
            viewModel.Comments = identification.Comments;
            viewModel.IsCustomIdentification = identification.IsCustomIdentification;
            viewModel.Taxonomy = identification.Taxonomy;
            viewModel.Kingdom = identification.TryGetRankName("kingdom");
            viewModel.Phylum = identification.TryGetRankName("phylum");
            viewModel.Class = identification.TryGetRankName("class");
            viewModel.Order = identification.TryGetRankName("order");
            viewModel.Family = identification.TryGetRankName("family");
            viewModel.Genus = identification.TryGetRankName("genus");
            viewModel.Species = identification.TryGetRankName("species");
            viewModel.Subspecies = identification.TryGetRankName("subspecies");
            viewModel.CommonGroupNames = identification.CommonGroupNames.ToList();
            viewModel.CommonNames = identification.CommonNames.ToList();
            viewModel.Synonyms = identification.Synonyms.ToList();

            return viewModel;
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
            viewModel.Comments = sightingNote.Comments;

            return viewModel;
        }

        public dynamic Make(Sighting sighting, SightingNote sightingNote, User user, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sightingNote.Id;
            viewModel.SightingId = sighting.Id;
            viewModel.CreatedOn = sightingNote.CreatedOn;
            viewModel.Comments = sightingNote.Comments;
            viewModel.Descriptions = sightingNote.Descriptions;
            viewModel.Tags = sightingNote.Tags;
            viewModel.User = _userViewFactory.Make(user);
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
            }

            return viewModel;
        }

        public dynamic Make(Sighting sighting, IdentificationNew identification, User user, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = identification.Id;
            viewModel.SightingId = sighting.Id;
            viewModel.CreatedOn = identification.CreatedOn;
            viewModel.Comments = identification.Comments;
            viewModel.AllCommonNames = identification.AllCommonNames;
            viewModel.Category = identification.Category;
            viewModel.CommonGroupNames = identification.CommonGroupNames;
            viewModel.CommonNames = identification.CommonNames;
            viewModel.IsCustomIdentification = identification.IsCustomIdentification;
            viewModel.Name = identification.Name;
            viewModel.RankName = identification.RankName;
            viewModel.RankType = identification.RankType;
            viewModel.Synonyms = identification.Synonyms;
            viewModel.Ranks = identification.TaxonomicRanks;
            viewModel.Taxonomy = identification.Taxonomy;
            viewModel.UpdateOn = identification.UpdateOn;
            viewModel.User = _userViewFactory.Make(user);
            viewModel.TotalVoteScore = identification.Votes.Sum(x => x.Score);

            // Current user-specific properties
            if (authenticatedUser != null)
            {
                var userId = authenticatedUser.Id;

                viewModel.UserVoteScore = identification.Votes.Any(x => x.User.Id == userId) ? identification.Votes.Single(x => x.User.Id == userId).Score : 0;
            }

            viewModel.CreatedOnDescription = identification.CreatedOn.ToString("d MMMM yyyy");

            return viewModel;
        }

        #endregion  
 
    }
}
