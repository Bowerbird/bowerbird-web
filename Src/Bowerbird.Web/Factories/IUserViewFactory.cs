using System;
namespace Bowerbird.Web.Factories
{
    public interface IUserViewFactory : IFactory
    {
        object Make(Bowerbird.Core.DomainModels.User user);
    }
}
