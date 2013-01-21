/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Core.Config;
using Bowerbird.Core.Indexes;

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

            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId == command.UserId)
                .First();

            var sighting = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .Where(x => x.ContributionId == command.SightingId && (x.ContributionType == "observation" || x.ContributionType == "record"))
                .First()
                .Contribution as Sighting;

            sighting.AddToFavourites(
                userResult.Groups.First(x => x.GroupType == "favourites") as Favourites, 
                userResult.User, 
                DateTime.UtcNow);

            _documentSession.Store(sighting);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}