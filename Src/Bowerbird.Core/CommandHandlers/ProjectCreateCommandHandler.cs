using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Fields

        private IRepository<Project> _projectRepository;
        private IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IRepository<Project> projectRepository,
            IRepository<User> userRepository
            )
        {
            Check.RequireNotNull(projectRepository, "projectRepository");
            Check.RequireNotNull(userRepository, "userRepository");

            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectCreateCommand projectCreateCommand)
        {
            Check.RequireNotNull(projectCreateCommand, "projectCreateCommand");

            var project = new Project(
                _userRepository.Load(projectCreateCommand.UserId),
                projectCreateCommand.Name,
                projectCreateCommand.Description
                );

            _projectRepository.Add(project);
        }

        #endregion
		
    }
}