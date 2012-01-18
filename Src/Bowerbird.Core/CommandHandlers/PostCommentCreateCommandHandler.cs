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
    using Bowerbird.Core.DomainModels.Comments;

    #endregion

    public class PostCommentCreateCommandHandler : ICommandHandler<PostCommentCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PostCommentCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCommentCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var postComment = new PostComment(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<Post>(command.PostId),
                command.PostedOn,
                command.Message
                );

            _documentSession.Store(postComment);
        }

        #endregion
    }
}