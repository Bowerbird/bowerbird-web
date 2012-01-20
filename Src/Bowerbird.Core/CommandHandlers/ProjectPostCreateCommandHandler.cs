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
using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectPostCreateCommandHandler : ICommandHandler<ProjectPostCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectPostCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostCreateCommand projectPostCreateCommand)
        {
            Check.RequireNotNull(projectPostCreateCommand, "projectPostCreateCommand");

            var projectPost = new ProjectPost(
                _documentSession.Load<Project>(projectPostCreateCommand.ProjectId),
                _documentSession.Load<User>(projectPostCreateCommand.UserId),
                projectPostCreateCommand.Timestamp,
                projectPostCreateCommand.Subject,
                projectPostCreateCommand.Message,
                _documentSession.Load<MediaResource>(projectPostCreateCommand.MediaResources).ToList()
                );

            _documentSession.Store(projectPost);
        }

        #endregion
    }
}