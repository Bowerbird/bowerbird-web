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

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class OrganisationPostCreateCommandHandler : ICommandHandler<OrganisationPostCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationPostCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(OrganisationPostCreateCommand organisationPostCreateCommand)
        {
            Check.RequireNotNull(organisationPostCreateCommand, "organisationPostCreateCommand");

            var organisationPost = new OrganisationPost(
                _documentSession.Load<Organisation>(organisationPostCreateCommand.OrganisationId),
                _documentSession.Load<User>(organisationPostCreateCommand.UserId),
                organisationPostCreateCommand.PostedOn,
                organisationPostCreateCommand.Subject,
                organisationPostCreateCommand.Message,
                _documentSession.Load<MediaResource>(organisationPostCreateCommand.MediaResources).ToList()
                );

            _documentSession.Store(organisationPost);
        }

        #endregion
    }
}