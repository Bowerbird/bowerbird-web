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
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationCreateCommandHandler : ICommandHandler<ObservationCreateCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods 

        public void Handle(ObservationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var mediaResourceIds = command.Media.Select(x => x.MediaResourceId);

            var mediaResources = _documentSession.Load<MediaResource>(mediaResourceIds);

            // TODO: Create any new media resources from scratch

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

            var observation = new Observation(
                user,
                command.Title,
                DateTime.UtcNow,
                command.ObservedOn,
                command.Latitude,
                command.Longitude,
                command.Address,
                command.IsIdentificationRequired,
                command.AnonymiseLocation,
                command.Category,
                userProject,
                projects);

            // Ensure at least one media set as primary
            if(command.Media.All(x => !x.IsPrimaryMedia))
            {
                command.Media.First().IsPrimaryMedia = true;
            }

            foreach (var media in command.Media)
            {
                // TODO: Create any new media resources from scratch

                observation.AddMedia(
                    mediaResources.Single(x => x.Id.ToLower() == media.MediaResourceId),
                    media.Description,
                    media.Licence,
                    media.IsPrimaryMedia);
            }

            _documentSession.Store(observation);
        }

        #endregion      
      
    }
}
