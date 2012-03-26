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
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

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

        public void Handle(ObservationCreateCommand observationCreateCommand)
        {
            Check.RequireNotNull(observationCreateCommand, "observationCreateCommand");

            var addMediaResources = (from mediaResource in _documentSession.Query<MediaResource>()
                                         where mediaResource.Id.In(observationCreateCommand.AddMediaResources.Keys)
                                         select new
                                             {
                                                 mediaResource,
                                                 description = observationCreateCommand.AddMediaResources.Single(x => x.Key == mediaResource.Id).Value
                                             })
                                             .ToDictionary(x => x.mediaResource, x => x.description);

            var observation = new Observation(
                _documentSession.Load<User>(observationCreateCommand.UserId),
                observationCreateCommand.Title,
                DateTime.Now,
                observationCreateCommand.ObservedOn,
                observationCreateCommand.Latitude,
                observationCreateCommand.Longitude,
                observationCreateCommand.Address,
                observationCreateCommand.IsIdentificationRequired,
                observationCreateCommand.Category,
                addMediaResources);

            /* if Observation is in project 
             * then create GroupContribution and add Observation
             * then find all groups in hierarchy 
             * then for each group in hierarchy
             * add group to GroupContribution
             */

            _documentSession.Store(observation);
        }

        #endregion      
      
    }
}
