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

            var mediaResourceIds = command.Media.Select(x => x.Item1);

            var mediaResources = _documentSession.Load<MediaResource>(mediaResourceIds);

            var userProject = _documentSession
                .Query<UserProject>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == command.UserId)
                .First();

            var user = _documentSession.Load<User>(command.UserId);

            var addMedia = (from mediaResource in mediaResources
                                     select new
                                     {
                                         mediaResource,
                                         description = command.Media.Single(x => x.Item1.ToLower() == mediaResource.Id.ToLower()).Item2,
                                         licence = command.Media.Single(x => x.Item1.ToLower() == mediaResource.Id.ToLower()).Item3
                                     })
                                     .Select(x => new Tuple<MediaResource, string, string>(x.mediaResource, x.description, x.licence));

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
                projects,
                addMedia);

            _documentSession.Store(observation);
        }

        #endregion      
      
    }
}
