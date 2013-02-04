using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface IPostViewFactory
    {
        object MakeCreatePost(string groupId);

        object Make(Post post, User user, Group group, User authenticatedUser);
    }
}
