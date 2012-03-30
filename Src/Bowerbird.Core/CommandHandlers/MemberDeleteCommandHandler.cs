/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Repositories;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class MemberDeleteCommandHandler : ICommandHandler<MemberDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(MemberDeleteCommand memberDeleteCommand)
        {
            Check.RequireNotNull(memberDeleteCommand, "memberDeleteCommand");

            var member = _documentSession.LoadMember(memberDeleteCommand.GroupId, memberDeleteCommand.UserId);

            _documentSession.Delete(member);
        }

        #endregion
    }
}