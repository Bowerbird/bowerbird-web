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
using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class OrganisationPostUpdateCommandHandler : ICommandHandler<OrganisationPostUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationPostUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(OrganisationPostUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var organisationPost = _documentSession.Load<OrganisationPost>(command.Id);

            organisationPost.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.Subject,
                command.Message,
                _documentSession.Load<MediaResource>(command.MediaResources));

            _documentSession.Store(organisationPost);
        }

        #endregion

    }
}