using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class Group_ById : AbstractMultiMapIndexCreationTask<Group>
    {
        public Group_ById()
        {
            AddMap<Organisation>(organisations => from organisation in organisations
                                                  select new
                                                             {
                                                                organisation.Name,
                                                                organisation.Description
                                                             });

            AddMap<Team>(teams => from team in teams
                                                select new
                                                {
                                                    team.Name,
                                                    team.Description
                                                });
        }
    }
}
