﻿using System;
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

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string ProjectId { get; set; }

        public string CreatedByUserId { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
