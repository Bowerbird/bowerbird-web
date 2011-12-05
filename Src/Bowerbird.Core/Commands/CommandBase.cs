using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bowerbird.Core.Commands
{
    public abstract class CommandBase : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool IsValid()
        {
            return ValidationResults().Count > 0;
        }

        public abstract ICollection<ValidationResult> ValidationResults();

        #endregion      
      
    }
}
