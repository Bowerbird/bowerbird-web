/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class CommentUpdateCommandHandler : ICommandHandler<CommentUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public CommentUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(CommentUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var contribution = _documentSession.Query<IContribution, All_Contributions>()
                .Where(x => x.Id == command.ContributionId)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .FirstOrDefault();

            ((IDiscussed)contribution).UpdateComment(
                command.Id,
                command.Comment,
                _documentSession.Load<User>(command.UserId),
                command.UpdatedOn);

            _documentSession.Store(contribution);
        }

        #endregion

    }
}