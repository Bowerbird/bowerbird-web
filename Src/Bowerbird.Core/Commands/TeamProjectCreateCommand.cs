using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class TeamProjectCreateCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public TeamProjectCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string TeamId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> Administrators { get; set; }

        public List<string> Members { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        private void InitMembers()
        {
            Administrators = new List<string>();

            Members = new List<string>();
        }

        #endregion
    }
}