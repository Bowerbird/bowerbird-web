using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class PostViewFactory : IPostViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public PostViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeNewPost(string groupId)
        {
            return new
            {
                Subject = string.Empty,
                Message = string.Empty,
                GroupId = groupId
            };
        }

        public dynamic Make(Post post, User user, Group group, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = post.Id;

            viewModel.Subject = post.Subject;
            viewModel.Message = post.Message;
            viewModel.PostType = post.PostType;
            viewModel.GroupId = post.Group.Id;
            viewModel.Group = _groupViewFactory.Make(group);
            viewModel.User = _userViewFactory.Make(user);
            viewModel.CreatedOnDescription = post.CreatedOn.ToString("d MMM yyyy");

            return viewModel;
        }

        #endregion  
 
    }
}
