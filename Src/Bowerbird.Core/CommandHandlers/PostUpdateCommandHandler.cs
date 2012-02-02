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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class PostUpdateCommandHandler : ICommandHandler<PostUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PostUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostUpdateCommand postUpdateCommand)
        {
            Check.RequireNotNull(postUpdateCommand, "postUpdateCommand");

            var post = _documentSession.Load<Post>(postUpdateCommand.Id);

            post.UpdateDetails(
                _documentSession.Load<User>(postUpdateCommand.UserId),
                postUpdateCommand.Subject,
                postUpdateCommand.Message,
                _documentSession.Load<MediaResource>(postUpdateCommand.MediaResources));

            _documentSession.Store(post);
        }

        #endregion

    }
}