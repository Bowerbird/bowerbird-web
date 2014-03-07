using System.Dynamic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class PostViewFactory : IPostViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public PostViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory
            )
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

        public object MakeCreatePost(string groupId)
        {
            return new
            {
                Subject = string.Empty,
                Message = string.Empty,
                GroupId = groupId
            };
        }

        public object Make(Post post, User user, Group group, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = post.Id;

            viewModel.Subject = post.Subject;
            viewModel.Message = post.Message;
            viewModel.PostType = post.PostType;
            viewModel.GroupId = post.Group.Id;
            viewModel.Group = _groupViewFactory.Make(group, authenticatedUser);
            viewModel.User = _userViewFactory.Make(user, authenticatedUser);
            viewModel.CreatedOn = post.CreatedOn;
            viewModel.CreatedOnDescription = post.CreatedOn.ToString("d MMM yyyy");

            return viewModel;
        }

        #endregion  
 
    }
}