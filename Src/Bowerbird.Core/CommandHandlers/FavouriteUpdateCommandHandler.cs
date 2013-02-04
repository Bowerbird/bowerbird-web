/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;

namespace Bowerbird.Core.CommandHandlers
{
    public class FavouriteUpdateCommandHandler : ICommandHandler<FavouriteUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public FavouriteUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(FavouriteUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var user = _documentSession.Load<User>(command.UserId);
            var favourites = _documentSession.Load<Favourites>(user.Favourites.Id);
            Sighting sighting = null;

            if (command.SightingId.ToLower().StartsWith("observation"))
            {
                sighting = _documentSession.Load<Observation>(command.SightingId);
            }
            else
            {
                sighting = _documentSession.Load<Record>(command.SightingId);
            }

            sighting.AddToFavourites(
                favourites, 
                user, 
                DateTime.UtcNow);

            _documentSession.Store(sighting);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}