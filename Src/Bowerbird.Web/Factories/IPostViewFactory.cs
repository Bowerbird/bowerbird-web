using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface IPostViewFactory : IFactory
    {
        object MakeCreatePost(string groupId);

        object Make(Post post, User user, Group group, User authenticatedUser);
    }
}
