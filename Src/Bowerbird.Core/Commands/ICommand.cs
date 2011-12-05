using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public interface ICommand
    {
        bool IsValid();

        ICollection<ValidationResult> ValidationResults();
    }
}
