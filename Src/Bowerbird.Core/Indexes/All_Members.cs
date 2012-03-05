using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_Members : AbstractMultiMapIndexCreationTask
    {
        public All_Members()
        {
            AddMap<GlobalMember>(globalMembers =>
                from globalMember in globalMembers
                select new
                {
                    globalMember.Id,
                    globalMember.Roles
                });

            AddMap<GroupMember>(groupMembers =>
                from groupMember in groupMembers
                select new
                {
                    groupMember.Id,
                    groupMember.Roles
                });

        }
    }
}