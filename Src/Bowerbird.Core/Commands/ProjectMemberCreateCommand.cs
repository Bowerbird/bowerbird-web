using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Commands
{
    public class ProjectMemberCreateCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public ProjectMemberCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string ProjectId { get; set; }

        public string CreatedByUserId { get; set; }

        public List<string> Roles { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        private void InitMembers()
        {
            Roles = new List<string>();
        }

        #endregion
    }
}
