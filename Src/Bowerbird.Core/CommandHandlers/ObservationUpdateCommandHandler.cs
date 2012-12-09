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
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationUpdateCommandHandler : ICommandHandler<ObservationUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// TODO: Add functionality to update MediaResources
        /// </summary>
        public void Handle(ObservationUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observation = _documentSession
                .Load<Observation>(command.Id);

            var mediaResourceIds = command.Media.Select(x => x.MediaResourceId);
            var mediaResources = _documentSession.Load<MediaResource>(mediaResourceIds);

            IEnumerable<Project> projects = new List<Project>();

            if (command.Projects != null && command.Projects.Count() > 0)
            {
                projects = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>()
                    .Where(x => x.GroupId.In(command.Projects))
                    .ToList()
                    .Select(x => x.Project);
            }

            // Ensure at least one media set as primary
            if (command.Media.All(x => !x.IsPrimaryMedia))
            {
                command.Media.First().IsPrimaryMedia = true;
            }

            observation.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow,
                command.Title,
                command.ObservedOn,
                command.Latitude,
                command.Longitude,
                command.Address,
                command.AnonymiseLocation,
                command.Category,
                projects,
                command.Media.Select(x =>
                    new Tuple<MediaResource, string, string, bool>(
                        mediaResources.Single(y => y.Id == x.MediaResourceId),
                        x.Description,
                        x.Licence,
                        x.IsPrimaryMedia)));

            _documentSession.Store(observation);
        }

        #endregion      
    }
}