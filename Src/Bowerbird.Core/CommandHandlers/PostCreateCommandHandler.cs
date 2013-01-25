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
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;

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

        public void Handle(PostCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var group = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == command.GroupId)
                .First()
                .Group;

            IEnumerable<MediaResource> mediaResources = new List<MediaResource>();

            if (command.MediaResources.Any())
            {
                mediaResources = _documentSession.Load<MediaResource>(command.MediaResources);
            }

            var post = new Post(
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow,
                command.Subject,
                command.Message,
                command.PostType,
                mediaResources,
                group);

            _documentSession.Store(post);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}