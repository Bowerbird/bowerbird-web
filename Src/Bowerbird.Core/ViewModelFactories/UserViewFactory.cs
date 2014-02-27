using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
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

        public object Make(User user, User authenticatedUser, bool fullDetails = false, int? sightingCount = 0, IEnumerable<Observation> sampleObservations = null, int? followerCount = 0)
        {
            Check.RequireNotNull(user, "user");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = user.Id;
            viewModel.Avatar = user.Avatar;
            viewModel.Name = user.Name;
            viewModel.LatestActivity = user.SessionLatestActivity;
            viewModel.LatestHeartbeat = user.SessionLatestHeartbeat;

            if (fullDetails)
            {
                viewModel.Joined = user.Joined.ToString("d MMM yyyy");
                viewModel.Description = user.Description;
                viewModel.ProjectCount = user.Memberships.Where(x => x.Group.GroupType == "project").Count();
                viewModel.OrganisationCount = user.Memberships.Where(x => x.Group.GroupType == "organisation").Count();
                viewModel.SightingCount = sightingCount;

                if (sampleObservations != null)
                {
                    viewModel.SampleObservations = sampleObservations.Select(x =>
                                                                    new
                                                                    {
                                                                        x.Id,
                                                                        Media = x.PrimaryMedia
                                                                    });
                }
                else
                {
                    viewModel.SampleObservations = new object[] { };
                }

                viewModel.FollowingCount = user.FollowingUsers.Count();
                viewModel.FollowerCount = followerCount;

                if (authenticatedUser != null)
                {
                    if (authenticatedUser.Id == user.Id)
                    {
                        viewModel.IsFollowing = false;
                        viewModel.IsFollowed = false;
                    }
                    else
                    {
                        viewModel.IsFollowing = authenticatedUser.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == user.UserProject.Id);
                        viewModel.IsFollowed = user.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == authenticatedUser.UserProject.Id);
                    }
                }
            }

            return viewModel;
        }

        #endregion   
    }
}