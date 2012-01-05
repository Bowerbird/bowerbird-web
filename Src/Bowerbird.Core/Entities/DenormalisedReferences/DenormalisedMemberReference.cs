using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;

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
            Check.RequireNotNull(member, "member");

            return new DenormalisedMemberReference
            {
                Type = SetMemberType(member),
                Id = member.Id,
                Roles = member.Roles.Select(x => x.Id)
            };
        }

        //private static string SetMemberId(Member member)
        //{
        //    if (member is TeamMember) return ((TeamMember) member).Team.Id;

        //    if (member is ProjectMember) return ((ProjectMember) member).Project.Id;

        //    return member.Id;
        //}

        private static string SetMemberType(Member member)
        {
            if (member is TeamMember) return "teammember";

            if (member is ProjectMember) return "projectmember";

            return "globalmember";
        }

        #endregion

    }
}
