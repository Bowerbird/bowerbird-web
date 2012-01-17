/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using System;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class ProjectMemberCreateCommandHandler : ICommandHandler<ProjectMemberCreateCommand>
    {
        #region Fields

        private readonly IRepository<ProjectMember> _projectMemberRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Role> _roleRepository;

        #endregion

        #region Constructors

        public ProjectMemberCreateCommandHandler(
            IRepository<ProjectMember> projectMemberRepository
            ,IRepository<User> userRepository
            ,IRepository<Project> projectRepository
            ,IRepository<Role> roleRepository
            )
        {
            Check.RequireNotNull(projectMemberRepository, "projectMemberRepository");
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(projectRepository, "projectRepository");
            Check.RequireNotNull(roleRepository, "roleRepository");

            _projectMemberRepository = projectMemberRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _roleRepository = roleRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectMemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectMember = new ProjectMember(
                _userRepository.Load(command.CreatedByUserId),
                _projectRepository.Load(command.ProjectId),
                _userRepository.Load(command.UserId),
                _roleRepository.Load(command.Roles)
                );

            _projectMemberRepository.Add(projectMember);
        }

        #endregion
		
    }
}