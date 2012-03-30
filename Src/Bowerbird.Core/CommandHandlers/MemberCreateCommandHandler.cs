/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class MemberCreateCommandHandler : ICommandHandler<MemberCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberCreateCommand memberCreateCommand)
        {
            Check.RequireNotNull(memberCreateCommand, "memberCreateCommand");

            var member = new Member(
                _documentSession.Load<User>(memberCreateCommand.CreatedByUserId),
                _documentSession.Load<User>(memberCreateCommand.UserId),
                _documentSession.Load<Group>(memberCreateCommand.GroupId),
                _documentSession.Query<Role>()
                    .Where(x => x.Id.In(memberCreateCommand.Roles))
                    .ToList()
                );

            _documentSession.Store(member);
        }

        #endregion
		
    }
}