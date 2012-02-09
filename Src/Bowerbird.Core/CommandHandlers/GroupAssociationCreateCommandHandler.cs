/* Bowerbird V1  - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class GroupAssociationCreateCommandHandler : ICommandHandler<GroupAssociationCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupAssociationCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(GroupAssociationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var parentGroup = _documentSession.Load<Group>(command.ParentGroupId);

            parentGroup.AddGroupAssociation(
                _documentSession.Load<Group>(command.ChildGroupId),
                _documentSession.Load<User>(command.UserId),
                command.CreatedDateTime
                );

            _documentSession.Store(parentGroup);

            _documentSession.SaveChanges();
        }

        #endregion
    }
}