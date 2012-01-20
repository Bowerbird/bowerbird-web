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

using System;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectUpdateCommandHandler : ICommandHandler<ProjectUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectUpdateCommand projectUpdateCommand)
        {
            Check.RequireNotNull(projectUpdateCommand, "projectUpdateCommand");

            var project = _documentSession.Load<Project>(projectUpdateCommand.Id);

            project.UpdateDetails(
                _documentSession.Load<User>(projectUpdateCommand.UserId),
                projectUpdateCommand.Name,
                projectUpdateCommand.Description
                );

            _documentSession.Store(project);
        }

        #endregion				
    }
}