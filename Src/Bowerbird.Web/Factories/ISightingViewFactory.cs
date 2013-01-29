using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface ISightingViewFactory : IFactory
    {
        object MakeCreateObservation(string category = "", string projectId = "");

        object MakeCreateRecord(string category = "", string projectId = "");

        object Make(Sighting sighting, User user, IEnumerable<Group> projects, User authenticatedUser);
    }
}
