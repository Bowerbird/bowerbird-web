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

using System;
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class SightingNoteUpdateCommandHandler : ICommandHandler<SightingNoteUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SightingNoteUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(SightingNoteUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var sighting = _documentSession
                .Load<dynamic>(command.SightingId) as Sighting;

            sighting.UpdateNote(
                command.Id,
                command.Tags.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()),
                command.Descriptions,
                command.Comments,
                DateTime.UtcNow,
                _documentSession.Load<User>(command.UserId));

            _documentSession.Store(sighting);
            _documentSession.SaveChanges();
        }

        #endregion

    }
}