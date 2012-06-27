/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ChatCreateCommandHandler : ICommandHandler<ChatCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ChatCreateCommandHandler(
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ChatCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            System.Diagnostics.Debug.WriteLine("Async Document Session: {0} HasChanges: {1}, NumberOfRequests: {2}.", ((Raven.Client.Document.DocumentSession)_documentSession).Id, _documentSession.Advanced.HasChanges, _documentSession.Advanced.NumberOfRequests);

            var createdByUser = _documentSession.Load<User>(command.CreatedByUserId);

            Chat chat = null;
            Group group = null;

            if (!string.IsNullOrWhiteSpace(command.GroupId))
            {
                group = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>()
                    .Where(x => x.GroupId == command.GroupId)
                    .ToList()
                    .Select(x => x.Group)
                    .First();
            }

            chat = new Chat(
                command.ChatId,
                createdByUser,
                _documentSession.Load<User>(command.UserIds),
                command.CreatedDateTime,
                group);

            _documentSession.Store(chat);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}