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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    #endregion

    public class SightingNoteCreateCommandHandler : ICommandHandler<SightingNoteCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SightingNoteCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(SightingNoteCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var sighting = _documentSession
                               .Query<All_Contributions.Result, All_Contributions>()
                               .AsProjection<All_Contributions.Result>()
                               .Where(x => x.ContributionId == command.SightingId)
                               .First()
                               .Contribution as Sighting;

            sighting.AddNote(
                command.CommonName,
                command.ScientificName,
                command.Taxonomy,
                command.Tags,
                command.Descriptions,
                command.References,
                command.NotedOn,
                _documentSession.Load<User>(command.UserId));

            _documentSession.Store(sighting);
        }

        #endregion

    }
}