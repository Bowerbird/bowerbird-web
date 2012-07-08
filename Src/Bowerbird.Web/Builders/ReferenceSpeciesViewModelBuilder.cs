///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Linq;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Indexes;
//using Bowerbird.Core.Paging;
//using Bowerbird.Web.ViewModels;
//using Raven.Client;
//using Raven.Client.Linq;
//using Bowerbird.Web.Factories;

//namespace Bowerbird.Web.Builders
//{
//    public class ReferenceSpeciesViewModelBuilder : IReferenceSpeciesViewModelBuilder
//    {
//        #region Fields

//        private readonly IDocumentSession _documentSession;
//        private readonly IUserViewFactory _userViewFactory;

//        #endregion

//        #region Constructors

//        public ReferenceSpeciesViewModelBuilder(
//            IDocumentSession documentSession,
//            IUserViewFactory userViewFactory)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");
//            Check.RequireNotNull(userViewFactory, "userViewFactory");

//            _documentSession = documentSession;
//            _userViewFactory = userViewFactory;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public object BuildReferenceSpecies(string referenceSpeciesId)
//        {
//            Check.RequireNotNullOrWhitespace(referenceSpeciesId, "referenceSpeciesId");

//            return MakeReferenceSpecies(_documentSession.Load<ReferenceSpecies>(referenceSpeciesId));
//        }

//        public object BuildReferenceSpeciesList(PagingInput pagingInput)
//        {
//            Check.RequireNotNull(pagingInput, "pagingInput");

//            RavenQueryStatistics stats;

//            return _documentSession
//                .Query<All_ReferenceSpecies.Result, All_ReferenceSpecies>()
//                .AsProjection<All_ReferenceSpecies.Result>()
//                .Include(x => x.ReferenceSpecies.SpeciesId)
//                .Include(x => x.ReferenceSpecies.User.Id)
//                .Statistics(out stats)
//                .Skip(pagingInput.GetSkipIndex())
//                .Take(pagingInput.PageSize)
//                .ToList()
//                .Select(x => MakeReferenceSpecies(x.ReferenceSpecies))
//                .ToPagedList(
//                    pagingInput.Page,
//                    pagingInput.PageSize,
//                    stats.TotalResults,
//                    null);
//        }

//        private object MakeReferenceSpecies(ReferenceSpecies referenceSpecies)
//        {
//            return new
//            {
//                referenceSpecies.Id,
//                referenceSpecies.CreatedDateTime,
//                referenceSpecies.GroupId,
//                referenceSpecies.SmartTags,
//                Creator = _userViewFactory.Make(_documentSession.Load<User>(referenceSpecies.User.Id)),
//                Species = MakeSpecies(_documentSession.Load<Species>(referenceSpecies.SpeciesId))
//            };
//        }

//        private static object MakeSpecies(Species species)
//        {
//            return new
//            {
//                species.Id,
//                species.Kingdom,
//                species.Order,
//                species.Group,
//                species.SpeciesName,
//                species.Taxonomy,
//                species.GenusName,
//                species.Family,
//                species.CommonNames
//            };
//        }

//        #endregion
//    }
//}