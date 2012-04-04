using System;
using Bowerbird.Web.ViewModels.Shared;
namespace Bowerbird.Web.Factories
{
    public interface IStreamItemFactory : IFactory
    {
        StreamItem Make(object item, string contributionType, string groupUserId, DateTime groupCreatedDateTime, string description);
    }
}
