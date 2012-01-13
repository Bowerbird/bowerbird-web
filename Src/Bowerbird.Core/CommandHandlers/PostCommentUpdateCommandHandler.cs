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

    #endregion

    public class PostCommentUpdateCommandHandler : ICommandHandler<PostCommentUpdateCommand>
    {
        #region Fields


        #endregion

        #region Constructors

        public PostCommentUpdateCommandHandler(
            //IDefaultRepository<User> userRepository
            )
        {
            //Check.RequireNotNull(userRepository, "userRepository");

            //_userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PostCommentUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}