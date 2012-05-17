/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamDeleteCommandHandler : ICommandHandler<TeamDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamDeleteCommandHandler(
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
        /// Remove Team from Organisations Descendants.
        /// Remove Team from Project's Ancestors.
        /// Remove Team.
        /// </summary>
        public void Handle(TeamDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var team = _documentSession.Load<Team>(command.Id);

            var organisation = _documentSession
                .Query<Organisation>()
                .Where(x => x.Descendants.Any(y => y.Id.ToLower() == team.Id))
                .FirstOrDefault();

            if (organisation != null)
            {
                organisation.RemoveDescendant(team);
                _documentSession.Store(organisation);
            }

            var projects = _documentSession
                .Query<Project>()
                .Where(x => x.Ancestry.Any(y => y.Id.ToLower() == team.Id))
                .ToList();

            if(projects.Count > 0)
            {
                foreach (var project in projects)
                {
                    project.Ancestry.ToList().RemoveAll(y => y.Id.ToLower() == team.Id);

                    _documentSession.Store(project);
                }
            }

            _documentSession.Delete(team);
        }

        #endregion

    }
}