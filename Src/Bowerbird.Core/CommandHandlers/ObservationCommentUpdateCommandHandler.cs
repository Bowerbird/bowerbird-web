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

    public class ObservationCommentUpdateCommandHandler : ICommandHandler<ObservationCommentUpdateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ObservationComment> _observationCommentRepository;

        #endregion

        #region Constructors

        public ObservationCommentUpdateCommandHandler(
            IRepository<User> userRepository
            , IRepository<ObservationComment> observationCommentRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(observationCommentRepository, "observationCommentRepository");

            _userRepository = userRepository;
            _observationCommentRepository = observationCommentRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCommentUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}