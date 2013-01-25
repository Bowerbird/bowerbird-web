/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using System;

namespace Bowerbird.Core.CommandHandlers
{
    public class MemberUpdateCommandHandler : ICommandHandler<MemberUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");
            
            var user = _documentSession.Load<User>(command.UserId);

            var group = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>()
                    .Where(x => x.GroupId == command.GroupId)
                    .First()
                    .Group;

            user.UpdateMembership(
                _documentSession.Load<User>(command.ModifiedByUserId),
                group,
                _documentSession.Load<Role>(command.Roles));

            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        #endregion

    }
}