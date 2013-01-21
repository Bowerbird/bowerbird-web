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
    public class CommentCreateCommandHandler : ICommandHandler<CommentCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public CommentCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(CommentCreateCommand command)
        {
            //Check.RequireNotNull(command, "command");

            //var discussable = _documentSession
            //    .Query<All_Contributions.Result, All_Contributions>()
            //    .AsProjection<All_Contributions.Result>()
            //    .Where(x => x.ContributionId == command.ContributionId)
            //    .First()
            //    .Contribution as IDiscussable;

            //discussable.Discussion.AddComment(
            //    command.Comment,
            //    _documentSession.Load<User>(command.UserId),
            //    command.CommentedOn,
            //    command.InReplyToCommentId);

            //_documentSession.Store(discussable);
        }

        #endregion

    }
}