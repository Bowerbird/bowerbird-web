using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Core.Repositories;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectPostDeleteCommandHandler : ICommandHandler<ProjectPostDeleteCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectPostDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostDeleteCommand projectPostDeleteCommand)
        {
            Check.RequireNotNull(projectPostDeleteCommand, "projectPostDeleteCommand");

            _documentSession.Delete(_documentSession.Load<ProjectPost>(projectPostDeleteCommand.Id));
        }

        #endregion
    }
}