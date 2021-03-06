﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class OrganisationUpdateCommandHandler : ICommandHandler<OrganisationUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(OrganisationUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var organisation = _documentSession.Load<Organisation>(command.Id);

            organisation.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                command.AvatarId != null ? _documentSession.Load<MediaResource>(command.AvatarId) : null,
                command.BackgroundId != null ? _documentSession.Load<MediaResource>(command.BackgroundId) : null,
                command.Categories);

            _documentSession.Store(organisation);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}