/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
    public class GroupAssociationDeleteCommandHandler : ICommandHandler<GroupAssociationDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupAssociationDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(GroupAssociationDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectObservation = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroup.Id == command.ParentGroupId && x.ChildGroup.Id == command.ChildGroupId)
                .FirstOrDefault();

            _documentSession.Delete(projectObservation);

            _documentSession.SaveChanges();
        }

        #endregion
    }
}