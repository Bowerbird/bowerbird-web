using System;
using Bowerbird.Web.ViewModels.Shared;
using System.Collections.Generic;
namespace Bowerbird.Web.Factories
{
    public interface IStreamItemFactory : IFactory
    {
        StreamItem Make(object item, IEnumerable<string> groups, string contributionType, string groupUserId, DateTime groupCreatedDateTime, string description);
    }
}
