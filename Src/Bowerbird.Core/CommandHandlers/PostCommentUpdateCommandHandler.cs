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
using Bowerbird.Core.DomainModels.Comments;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class PostCommentUpdateCommandHandler : ICommandHandler<PostCommentUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PostCommentUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCommentUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var postComment = _documentSession.Load<PostComment>(command.Id);

            postComment.UpdateCommentDetails(
                _documentSession.Load<User>(command.UserId),
                command.UpdatedOn,
                command.Comment);

            _documentSession.Store(postComment);
        }

        #endregion

    }
}