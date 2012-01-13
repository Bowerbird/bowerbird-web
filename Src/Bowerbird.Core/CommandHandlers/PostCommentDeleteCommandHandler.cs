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

using Bowerbird.Core.DomainModels.Comments;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class PostCommentDeleteCommandHandler : ICommandHandler<PostCommentDeleteCommand>
    {
        #region Fields

        private readonly IRepository<PostComment> _postCommentRepository;

        #endregion

        #region Constructors

        public PostCommentDeleteCommandHandler(
            IRepository<PostComment> postCommentRepository
            )
        {
            Check.RequireNotNull(postCommentRepository, "postCommentRepository");

            _postCommentRepository = postCommentRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCommentDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}