using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ProjectPostDeleteCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public ProjectPostDeleteCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public virtual string UserId { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {

        }

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}