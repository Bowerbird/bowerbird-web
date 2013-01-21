using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
namespace Bowerbird.Web.Factories
{
    public interface ISightingViewFactory : IFactory
    {
        object MakeNewObservation(string category = "", string projectId = "");

        object MakeNewRecord(string category = "", string projectId = "");

        //dynamic Make(All_Contributions.Result result, User authenticatedUser);

        dynamic Make(Sighting sighting, User user, IEnumerable<Group> projects, User authenticatedUser);
    }
}
