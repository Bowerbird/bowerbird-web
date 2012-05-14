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
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class OrganisationDeleteCommandHandler : ICommandHandler<DeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationDeleteCommandHandler(
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
        /// Remove Organisation from all Team's Ancestors
        /// Remove Organisation from all Project's Ancestors
        /// Remove Organisation
        /// </summary>
        public void Handle(DeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var organisation = _documentSession.Load<Organisation>(command.Id);

            var teams = _documentSession
               .Query<Team>()
               .Where(x => x.Ancestry.Any(y => y.ToLower() == organisation.Id))
               .ToList();

            if (teams.Count > 0)
            {
                foreach (var team in teams)
                {
                    team.Ancestry.ToList().RemoveAll(y => y.ToLower() == organisation.Id);
                    _documentSession.Store(team);
                }

                var projects = _documentSession
                   .Query<Project>()
                   .Where(x => x.Ancestry.Any(y => y.ToLower() == organisation.Id))
                   .ToList();

                if (projects.Count > 0)
                {
                    foreach (var project in projects)
                    {
                        project.Ancestry.ToList().RemoveAll(y => y.ToLower() == organisation.Id);
                        _documentSession.Store(project);
                    }
                }
            }

            _documentSession.Delete(organisation);
        }

        #endregion
    }
}