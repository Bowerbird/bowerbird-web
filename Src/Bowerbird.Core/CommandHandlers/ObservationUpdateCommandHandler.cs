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

        public void Handle(ObservationUpdateCommand observationUpdateCommand)
        {
            Check.RequireNotNull(observationUpdateCommand, "observationUpdateCommand");

            var observation = _documentSession.Load<Observation>(observationUpdateCommand.Id);

            var observationMediaItems = (from mediaResource in _documentSession.Query<MediaResource>()
                                         where mediaResource.Id.In(observationUpdateCommand.ObservationMediaItems.Keys)
                                         select new
                                             {
                                                 mediaResource,
                                                 description = observationUpdateCommand.ObservationMediaItems.Single(x => x.Key == mediaResource.Id).Value
                                             })
                                             .ToDictionary(x => x.mediaResource, x => x.description);
             
            observation.UpdateDetails(
                _documentSession.Load<User>(observationUpdateCommand.UserId),
                observationUpdateCommand.Title,
                observationUpdateCommand.ObservedOn,
                observationUpdateCommand.Latitude,
                observationUpdateCommand.Longitude,
                observationUpdateCommand.Address,
                observationUpdateCommand.IsIdentificationRequired,
                observationUpdateCommand.ObservationCategory,
                observationMediaItems);

            _documentSession.Store(observation);
        }

        #endregion      
    }
}