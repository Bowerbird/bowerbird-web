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

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectObservationDeleteCommandHandler : ICommandHandler<ProjectObservationDeleteCommand>
    {
        #region Members

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

        public void Handle(ProjectObservationDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectObservation = _documentSession
                .Query<ProjectObservation>()
                .Where(x => x.Project.Id == command.ProjectId && x.Observation.Id == command.ObservationId);

            _documentSession.Delete(projectObservation);

            _documentSession.SaveChanges();
        }

        #endregion
    }
}