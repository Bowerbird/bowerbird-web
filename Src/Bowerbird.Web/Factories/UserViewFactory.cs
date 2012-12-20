/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Dynamic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Factories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(User user)
        {
            Check.RequireNotNull(user, "user");

            return MakeBaseUser(user);
        }

        public object Make(All_Users.Result result, bool fullDetails = false)
        {
            Check.RequireNotNull(result, "result");

            dynamic viewModel = MakeBaseUser(result.User);

            if (fullDetails)
            {
                viewModel.Joined = result.User.Joined.ToString("d MMM yyyy");
                viewModel.Description = result.User.Description;
                viewModel.ProjectCount = result.User.Memberships.Where(x => x.Group.GroupType == "project").Count();
                viewModel.OrganisationCount = result.User.Memberships.Where(x => x.Group.GroupType == "organisation").Count();
                viewModel.SightingCount = result.SightingCount;
            }

            return viewModel;
        }

        private dynamic MakeBaseUser(User user)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = user.Id;
            viewModel.Avatar = user.Avatar;
            viewModel.Name = user.Name;
            viewModel.LatestActivity = user.SessionLatestActivity;
            viewModel.LatestHeartbeat = user.SessionLatestHeartbeat;

            return viewModel;
        }

        #endregion   
    }
}