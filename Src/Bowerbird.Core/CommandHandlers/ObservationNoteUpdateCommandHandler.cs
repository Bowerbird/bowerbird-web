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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationNoteUpdateCommandHandler : ICommandHandler<ObservationNoteUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationNoteUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationNoteUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observationNote = _documentSession.Load<ObservationNote>(command.Id);

            observationNote.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.CommonName,
                command.ScientificName,
                command.Taxonomy,
                command.Tags,
                command.Descriptions,
                command.References);

            _documentSession.Store(observationNote);
        }

        #endregion

    }
}