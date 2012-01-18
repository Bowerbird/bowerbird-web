/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Raven.Client;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectObservationDeleteCommandHandler : ICommandHandler<ProjectObservationDeleteCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectObservationDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(ProjectObservationDeleteCommand projectObservationDeleteCommand)
        {
            Check.RequireNotNull(projectObservationDeleteCommand, "projectObservationDeleteCommand");

            throw new NotImplementedException();
        }

        #endregion
    }
}