using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;

namespace Bowerbird.Core
{
    public interface ICommandProcessor
    {
        void Process<TCommand>(TCommand command) where TCommand : ICommand;

        IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand;

        void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler) where TCommand : ICommand;
    }
}
