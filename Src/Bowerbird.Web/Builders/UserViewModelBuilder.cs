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
    public class UserViewModelBuilder : IUserViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _userViewFactory.Make(_documentSession.Load<User>(idInput.Id));
        }

        public object BuildList(UserListInput listInput)
        {
            Check.RequireNotNull(listInput, "listInput");

            return BuildUser(listInput);
        }

        private object BuildUser(UserListInput listInput)
        {
            var userMemberships = _documentSession.Query<All_UserMemberships.Result, All_UserMemberships>()
                .Customize(x => x.Include(listInput.GroupId))
                .Where(x => x.GroupId == listInput.GroupId)
                .Distinct()
                .ToList();

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<User>()
                .Where(x => x.Id.In(userMemberships.Select(y => y.UserId)))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _userViewFactory.Make(x));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Users = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}