using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Factories
{
    public interface IBrowseItemFactory
    {
        object Make(object item, IEnumerable<string> groups, string contributionType, User groupUser, DateTime groupCreatedDateTime, string description);
    }
}