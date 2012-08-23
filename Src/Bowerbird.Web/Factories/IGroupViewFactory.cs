using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface IGroupViewFactory : IFactory
    {
        object Make(Group group);

        object Make(All_Groups.Result result);

        object Make(Group group, int memberCount, int observationCount, int postCount);
    }
}
