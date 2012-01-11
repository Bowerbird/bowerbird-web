using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ProjectPostCreateCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public ProjectPostCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string ProjectId { get; set; }

        public string UserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public IList<string> MediaResources { get; set; }

        public DateTime Timestamp { get; set; }

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