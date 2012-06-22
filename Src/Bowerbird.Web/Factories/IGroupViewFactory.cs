using System;
using Bowerbird.Core.DomainModels;
namespace Bowerbird.Web.Factories
{
    public interface IGroupViewFactory : IFactory
    {
        object Make(Group user);
    }
}
