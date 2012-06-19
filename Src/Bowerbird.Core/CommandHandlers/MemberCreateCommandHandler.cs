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
    public class MemberCreateCommandHandler : ICommandHandler<MemberCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var result = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == command.GroupId)
                .FirstOrDefault();
            
            var user = _documentSession.Load<User>(command.UserId);
            
            var createdByUser = _documentSession.Load<User>(command.CreatedByUserId);
            
            // TODO: FIX THIS HACK - ALL VERY VERBOSE - IN A HURRY..
            var roles = new List<Role>();
            Role role;

            if (command.GroupId.Contains("projects/"))
            {
                role = _documentSession.Load<Role>("roles/projectmember");
                roles.Add(role);
            }
            else if(command.GroupId.Contains("teams/"))
            {
                role = _documentSession.Load<Role>("roles/teammember");
                roles.Add(role);
            }
            else if(command.GroupId.Contains("organisations/"))
            {
                role = _documentSession.Load<Role>("roles/orgnaisationmember");
                roles.Add(role);
            }
            
            var member = new Member(createdByUser, user, result.Group, roles, true);

            _documentSession.Store(member);
        }

        #endregion

    }
}