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
            Check.RequireNotNull(command, "command");

            var contribution = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == command.ContributionId)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .FirstOrDefault();

            if (contribution == null) return;

            switch (contribution.ContributionType)
            {
                case "observation":
                    {
                        if (!string.IsNullOrWhiteSpace(command.InReplyToCommentId))
                        {
                            var inReplyToComment = contribution
                                .Observation
                                .Discussion
                                .Comments
                                .Where(x => x.Id == command.InReplyToCommentId)
                                .FirstOrDefault();

                            AddThreadedComment(contribution.Observation, command, inReplyToComment);
                        }
                        else
                        {
                            AddComment(contribution.Observation, command);
                        }
                    }
                    break;

                case "post":
                    {
                        if (!string.IsNullOrWhiteSpace(command.InReplyToCommentId))
                        {
                            var inReplyToComment = contribution
                                .Observation
                                .Discussion
                                .Comments
                                .Where(x => x.Id == command.InReplyToCommentId)
                                .FirstOrDefault();

                            AddThreadedComment(contribution.Post, command, inReplyToComment);
                        }
                        else
                        {
                            AddComment(contribution.Post, command);
                        }
                    }
                    break;
                default:
                    {
                        return;
                    }
            }

            _documentSession.Store(contribution);
        }

        private void AddComment(IDiscussed contribution, CommentCreateCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.InReplyToCommentId))
            {
                contribution.AddComment(
                    command.Comment,
                    _documentSession.Load<User>(command.UserId),
                    command.CommentedOn
                    );
            }
        }

        private void AddThreadedComment(IDiscussed contribution, CommentCreateCommand command, Comment comment)
        {
            contribution.AddThreadedComment(
                    command.Comment,
                    _documentSession.Load<User>(command.UserId),
                    command.CommentedOn,
                    comment
                    );
        }

        #endregion

    }
}