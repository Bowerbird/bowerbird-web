using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupSystemCommandHandler : ICommandHandler<SetupSystemCommand>
    {

        #region Members

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Permission> _permissionRepository;

        #endregion

        #region Constructors

        public SetupSystemCommandHandler(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<Permission> permissionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        #endregion

        #region Properties

        private List<Permission> Permissions { get; set; }
        
        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        #endregion

        #region Methods

        public void Handle(SetupSystemCommand setupSystemCommand)
        {
            Check.RequireNotNull(setupSystemCommand, "setupSystemCommand");

            Permissions = new List<Permission>();
            Roles = new List<Role>();
            Users = new List<User>();

            try
            {

                // Permmissions
                AddPermission("editorganisations", "Edit Organisations", "Ability to create, update and delete organisations");
                AddPermission("editteams", "Edit Teams", "Ability to create, update and delete teams");
                AddPermission("editprojects", "Edit Projects", "Ability to create, update and delete projects");
                AddPermission("edituserobservations", "Edit Observations", "Ability to create, update and delete own observations");
                AddPermission("editteamobservations", "Edit Team Observations", "Ability to create, update and delete team observations");
                AddPermission("editprojectobservations", "Edit Project Observations", "Ability to create, update and delete project observations");

                // Roles
                AddRole("globaladministrator", "Global Administrator", "Administrator across entire system.",
                    "editorganisations");
                AddRole("globalmember", "Global Member", "Member of the system.", "global",
                    "edituserobservations");
                AddRole("organisationadministrator", "Organisation Administrator", "Administrator of an organisation.",
                    "editteams");
                AddRole("teamadministrator", "Team Administrator", "Administrator of a team.",
                    "editprojects");
                AddRole("teammember", "Team Member", "Member of a team.",
                    "editteamobservations");
                AddRole("projectadministrator", "Project Administrator", "Administrator of a project.");
                AddRole("projectmember", "Project Member", "Member of a project.",
                    "editprojectobservations");

                // Users
                AddUser("frankr", "password", "frank@radocaj.com", "Frank", "Radocaj",
                    "globaladministrator", "globalmember");
                AddUser("hamishc", "password", "hcrittenden@museum.vic.gov.au", "Hamish", "Crittenden",
                    "globaladministrator", "globalmember");
                AddUser("kenw", "password", "kwalker@museum.vic.gov.au", "Ken", "Walker",
                    "globaladministrator", "globalmember");
            }
            finally
            {
                Permissions = null;
                Roles = null;
                Users = null;
            }
        }

        private void AddPermission(string id, string name, string description)
        {
            var permission = new Permission(id, name, description);

            _permissionRepository.Add(permission);

            Permissions.Add(permission);
        }

        private void AddRole(string id, string name, string description, params string[] permissionIds)
        {
            var permissions = Permissions.Where(x => permissionIds.Any(y => x.Id == "permissions/" + y));

            var role = new Role(id, name, description, permissions);

            _roleRepository.Add(role);

            Roles.Add(role);
        }

        private void AddUser(string username, string password, string email, string firstname, string lastname, params string[] roleIds)
        {
            var roles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));

            var user = new User(username, password, email, firstname, lastname, string.Empty, roles);

            _userRepository.Add(user);

            Users.Add(user);
        }

        #endregion      
      
    }
}
