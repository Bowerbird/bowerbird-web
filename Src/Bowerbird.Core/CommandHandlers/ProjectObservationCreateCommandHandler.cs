/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ProjectObservationCreateCommandHandler : ICommandHandler<ProjectObservationCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectObservationCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(ProjectObservationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectObservation = new ProjectObservation(
                _documentSession.Load<User>(command.UserId),
                command.CreatedDateTime,
                _documentSession.Load<Project>(command.ProjectId),
                _documentSession.Load<Observation>(command.ObservationId)
                );

            _documentSession.Store(projectObservation);
        }

        #endregion
    }
}