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
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class RecordCreateCommandHandler : ICommandHandler<RecordCreateCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public RecordCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods 

        public void Handle(RecordCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var userProject = _documentSession
                .Query<UserProject>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == command.UserId)
                .First();

            var user = _documentSession.Load<User>(command.UserId);

            var projects = new List<Project>();

            if (command.Projects != null && command.Projects.Count() > 0)
            {
                projects = _documentSession
                    .Query<Project>()
                    .Where(x => x.Id.In(command.Projects))
                    .ToList();
            }

            var record = new Record(
                command.Key,
                user,
                DateTime.UtcNow,
                command.ObservedOn,
                command.Latitude,
                command.Longitude,
                command.AnonymiseLocation,
                command.Category,
                userProject,
                projects);

            _documentSession.Store(record);
        }

        #endregion      
      
    }
}
