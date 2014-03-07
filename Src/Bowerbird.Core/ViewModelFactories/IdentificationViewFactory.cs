using System.Dynamic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class IdentificationViewFactory : IIdentificationViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public IdentificationViewFactory(
            IUserViewFactory userViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _userViewFactory = userViewFactory;
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
                Taxonomy = string.Empty,
                IdentificationComments = string.Empty
            };
        }

        public object MakeUpdateIdentification(Sighting sighting, User user, int identificationId)
        {
            var identification = sighting.Identifications.Single(x => x.SequentialId == identificationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = identificationId;
            viewModel.SightingId = sighting.Id;
            viewModel.IdentificationComments = identification.Comments;
            viewModel.Category = identification.Category;
            viewModel.Name = identification.Name;
            viewModel.RankName = identification.RankName;
            viewModel.RankType = identification.RankType;
            viewModel.IsCustomIdentification = identification.IsCustomIdentification;
            viewModel.AllCommonNames = identification.AllCommonNames;
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

        public object Make(Sighting sighting, IdentificationNew identification, User user, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = identification.Id;
            viewModel.SightingId = sighting.Id;
            viewModel.CreatedOn = identification.CreatedOn;
            viewModel.IdentificationComments = identification.Comments;
            viewModel.Category = identification.Category;
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
            viewModel.User = _userViewFactory.Make(user, authenticatedUser);
            viewModel.TotalVoteScore = identification.Votes.Sum(x => x.Score);

            // Current user-specific properties
            if (authenticatedUser != null)
            {
                var userId = authenticatedUser.Id;

                viewModel.UserVoteScore = identification.Votes.Any(x => x.User.Id == userId) ? identification.Votes.Single(x => x.User.Id == userId).Score : 0;
                viewModel.IsOwner = sighting.User.Id == authenticatedUser.Id;
            }

            viewModel.CreatedOnDescription = identification.CreatedOn.ToString("d MMMM yyyy");

            return viewModel;
        }

        #endregion  
 
    }
}