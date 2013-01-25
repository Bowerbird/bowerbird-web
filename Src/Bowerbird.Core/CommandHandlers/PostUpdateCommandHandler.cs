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
using System.Collections.Generic;
using System.Linq;
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

        public void Handle(PostUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var post = _documentSession.Load<Post>(command.Id);

            IEnumerable<MediaResource> mediaResources = new List<MediaResource>();

            if (command.MediaResources.Any())
            {
                mediaResources = _documentSession.Load<MediaResource>(command.MediaResources);
            }

            post.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.Subject,
                command.Message,
                command.PostType,
                mediaResources);

            _documentSession.Store(post);
            _documentSession.SaveChanges();
        }

        #endregion

    }
}