/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using System.Linq;

    using Commands;
    using DesignByContract;
    using DomainModels;
    using Repositories;

    #endregion

    public class ProjectPostUpdateCommandHandler : ICommandHandler<ProjectPostUpdateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectPostUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectPost = _documentSession.Load<ProjectPost>(command.Id);

            projectPost.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.Subject,
                command.Message,
                _documentSession.Load<MediaResource>(command.MediaResources));

            _documentSession.Store(projectPost);
        }

        #endregion

    }
}