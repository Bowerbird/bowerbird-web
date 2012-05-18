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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class ReferenceSpeciesViewModelBuilder : IReferenceSpeciesViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ReferenceSpeciesViewModelBuilder(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildReferenceSpecies(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return MakeReferenceSpecies(_documentSession.Load<ReferenceSpecies>(idInput.Id));
        }

        public object BuildReferenceSpeciesList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_ReferenceSpecies.Result, All_ReferenceSpecies>()
                .AsProjection<All_ReferenceSpecies.Result>()
                .Include(x => x.ReferenceSpecies.SpeciesId)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeReferenceSpecies(x.ReferenceSpecies))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Group.Id
        /// </summary>
        public object BuildGroupReferenceSpeciesList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_ReferenceSpecies.Result, All_ReferenceSpecies>()
                .AsProjection<All_ReferenceSpecies.Result>()
                .Where(x => x.GroupId == pagingInput.Id)
                .Include(x => x.ReferenceSpecies.SpeciesId)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeReferenceSpecies(x.ReferenceSpecies))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        private object MakeReferenceSpecies(ReferenceSpecies referenceSpecies)
        {
            return new
            {
                referenceSpecies.Id,
                referenceSpecies.CreatedDateTime,
                referenceSpecies.GroupId,
                referenceSpecies.SmartTags,
                Creator = MakeUser(referenceSpecies.User.Id),
                Species = MakeSpecies(_documentSession.Load<Species>(referenceSpecies.SpeciesId))
            };
        }

        private object MakeUser(string userId)
        {
            return MakeUser(_documentSession.Load<User>(userId));
        }

        private object MakeUser(User user)
        {
            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        private static object MakeSpecies(Species species)
        {
            return new
            {
                species.Id,
                species.Kingdom,
                species.Order,
                species.Group,
                species.SpeciesName,
                species.Taxonomy,
                species.GenusName,
                species.Family,
                species.CommonNames
            };
        }

        #endregion
    }
}