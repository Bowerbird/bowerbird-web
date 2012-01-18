﻿using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserCreateCommand userCreateCommand)
        {
            Check.RequireNotNull(userCreateCommand, "userCreateCommand");

            var user = new User(
                userCreateCommand.Password,
                userCreateCommand.Email,
                userCreateCommand.FirstName,
                userCreateCommand.LastName,
                _documentSession.Load<Role>(userCreateCommand.Roles));

            _documentSession.Store(user);
        }

        #endregion      
      
    }
}
