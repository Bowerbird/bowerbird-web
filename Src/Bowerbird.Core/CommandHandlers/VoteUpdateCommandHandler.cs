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
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.CommandHandlers
{
    public class VoteUpdateCommandHandler : ICommandHandler<VoteUpdateCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public VoteUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods 

        public void Handle(VoteUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            IVotable votable;

            votable = _documentSession
                         .Query<All_Contributions.Result, All_Contributions>()
                         .Where(x => x.ParentContributionId == command.ContributionId)
                         .ToList()
                         .First()
                         .Contribution as IVotable;

            votable.UpdateVote(
                command.Score,
                DateTime.UtcNow,
                _documentSession.Load<User>(command.UserId),
                command.SubContributionId);

            _documentSession.Store(votable);
            _documentSession.SaveChanges();
        }

        #endregion      
      
    }
}
