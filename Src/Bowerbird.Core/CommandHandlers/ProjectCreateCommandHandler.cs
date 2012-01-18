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

using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectCreateCommand projectCreateCommand)
        {
            Check.RequireNotNull(projectCreateCommand, "projectCreateCommand");

            var project = new Project(
                _documentSession.Load<User>(projectCreateCommand.UserId),
                projectCreateCommand.Name,
                projectCreateCommand.Description
                );

            _documentSession.Store(project);
        }

        #endregion
		
    }
}