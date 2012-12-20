using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface IGroupViewFactory : IFactory
    {
        dynamic Make(Group group);

        dynamic Make(All_Groups.Result result, bool fullDetails = false);
    }
}
