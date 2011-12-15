using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Entities.DenormalisedReferences
{
    public class DenormalisedMemberReference
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Type { get; private set; }

        public string Id { get; private set; }

        public IEnumerable<string> Roles { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedMemberReference(Member member)
        {
            return new DenormalisedMemberReference
            {
                Type = member.GetType().Name.ToLower(),
                Id = member is TeamMember ? ((TeamMember)member).Team.Id : ((ProjectMember)member).Project.Id,
                Roles = member.Roles.Select(x => x.Id)
            };
        }

        #endregion

    }
}
