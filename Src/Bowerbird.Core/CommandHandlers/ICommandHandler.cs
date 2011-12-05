using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;

namespace Bowerbird.Core.CommandHandlers
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand
    {
        TResult Handle(TCommand command);
    }
}
