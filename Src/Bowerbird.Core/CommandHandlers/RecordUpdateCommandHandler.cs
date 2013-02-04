/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class RecordUpdateCommandHandler : ICommandHandler<RecordUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public RecordUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(RecordUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            //var record = _documentSession
            //    .Load<Record>(command.Id);

            //record.UpdateDetails(
            //    _documentSession.Load<User>(command.UserId),
            //    command.ObservedOn,
            //    command.Latitude,
            //    command.Longitude,
            //    command.AnonymiseLocation,
            //    command.Category
            //);

            //_documentSession.Store(record);
        }

        #endregion      
    }
}