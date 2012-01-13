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

    public class ObservationCommentCreateCommandHandler : ICommandHandler<ObservationCommentCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Observation> _observationRepository;
        private readonly IRepository<ObservationComment> _observationCommentRepository;

        #endregion

        #region Constructors

        public ObservationCommentCreateCommandHandler(
            IRepository<User> userRepository
            , IRepository<Observation> observationRepository
            , IRepository<ObservationComment> observationCommentRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(observationRepository, "observationRepository");
            Check.RequireNotNull(observationCommentRepository, "observationCommentRepository");

            _userRepository = userRepository;
            _observationRepository = observationRepository;
            _observationCommentRepository = observationCommentRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCommentCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observationComment = new ObservationComment(
                _userRepository.Load(command.UserId),
                _observationRepository.Load(command.ObservationId),
                command.CommentedOn,
                command.Comment
                );

            _observationCommentRepository.Add(observationComment);
        }

        #endregion

    }
}