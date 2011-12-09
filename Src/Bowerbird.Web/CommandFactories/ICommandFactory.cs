using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.CommandFactories
{
    public interface ICommandFactory<TInput, TCommand>
    {
        TCommand Make(TInput input);
    }
}
