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

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<PostComment> _postCommentRepository;

        #endregion

        #region Constructors

        public PostCommentCreateCommandHandler(
             IRepository<User> userRepository
            ,IRepository<Post> postRepository
            , IRepository<PostComment> postCommentRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(postRepository, "postRepository");
            Check.RequireNotNull(postCommentRepository, "postCommentRepository");

            _userRepository = userRepository;
            _postRepository = postRepository;
            _postCommentRepository = postCommentRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCommentCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var postComment = new PostComment(
                _userRepository.Load(command.UserId),
                _postRepository.Load(command.PostId),
                command.PostedOn,
                command.Message
                );

            _postCommentRepository.Add(postComment);
        }

        #endregion
    }
}