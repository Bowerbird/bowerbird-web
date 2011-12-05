using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>
    {

        #region Members

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;

        #endregion

        #region Constructors

        public UserCreateCommandHandler(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserCreateCommand userCreateCommand)
        {
            Check.RequireNotNull(userCreateCommand, "userCreateCommand");

            var user = new User(
                userCreateCommand.Username,
                userCreateCommand.Password,
                userCreateCommand.Email,
                userCreateCommand.FirstName,
                userCreateCommand.LastName,
                userCreateCommand.Description,
                _roleRepository.Load(userCreateCommand.Roles));

            _userRepository.Add(user);
        }

        #endregion      
      
    }
}
