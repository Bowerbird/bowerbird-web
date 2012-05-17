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
        public void Handle(ObservationUpdateCommand observationUpdateCommand)
        {
            Check.RequireNotNull(observationUpdateCommand, "observationUpdateCommand");

            var observation = _documentSession
                .Load<Observation>(observationUpdateCommand.Id);

            var mediaItemsToAdd = observationUpdateCommand
                .AddMediaResources
                .Select(addMediaResource => new Tuple<MediaResource, string, string>(
                    _documentSession.Load<MediaResource>(addMediaResource.Item1), 
                    addMediaResource.Item2, 
                    addMediaResource.Item3))
                .ToList();

            observation.UpdateDetails(
                _documentSession.Load<User>(observationUpdateCommand.UserId),
                observationUpdateCommand.Title,
                observationUpdateCommand.ObservedOn,
                observationUpdateCommand.Latitude,
                observationUpdateCommand.Longitude,
                observationUpdateCommand.Address,
                observationUpdateCommand.IsIdentificationRequired,
                observationUpdateCommand.AnonymiseLocation,
                observationUpdateCommand.Category
            );

            foreach (var observationToAdd in mediaItemsToAdd)
            {
                observation.AddMedia(observationToAdd.Item1, observationToAdd.Item2, observationToAdd.Item3);
            }

            foreach (var removeMediaResource in observationUpdateCommand.RemoveMediaResources)
            {
                observation.RemoveMedia(removeMediaResource);
            }

            _documentSession.Store(observation);
        }

        #endregion      
    }
}