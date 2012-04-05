using System;
using Bowerbird.Web.ViewModels.Shared;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
namespace Bowerbird.Web.Factories
{
    public interface IStreamItemFactory : IFactory
    {
        StreamItem Make(object item, IEnumerable<string> groups, string contributionType, User groupUser, DateTime groupCreatedDateTime, string description);
    }
}
