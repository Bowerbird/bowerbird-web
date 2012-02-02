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

namespace Bowerbird.Core.CommandHandlers
{
    public class PostCreateCommandHandler : ICommandHandler<PostCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PostCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCreateCommand postCreateCommand)
        {
            Check.RequireNotNull(postCreateCommand, "postCreateCommand");

            var projectPost = new Post(
                _documentSession.Load<User>(postCreateCommand.UserId),
                postCreateCommand.Timestamp,
                postCreateCommand.Subject,
                postCreateCommand.Message,
                _documentSession.Load<MediaResource>(postCreateCommand.MediaResources).ToList()
                );

            // TODO: Add saving of GroupContribution

            _documentSession.Store(projectPost);
        }

        #endregion
    }
}