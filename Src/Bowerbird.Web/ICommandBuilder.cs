using System;
namespace Bowerbird.Web
{
    public interface ICommandBuilder
    {
        TCommand Build<TInput, TCommand>(TInput input, Action<TCommand> setup = null);
    }
}
