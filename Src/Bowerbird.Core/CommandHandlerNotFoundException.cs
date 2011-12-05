using System;

namespace Bowerbird.Core
{
    public class CommandHandlerNotFoundException : Exception
    {

        #region Members

        #endregion

        #region Constructors

        public CommandHandlerNotFoundException(Type type)
            : base(string.Format("Command handler not found for command type: {0}", type))
        {
        }

        public CommandHandlerNotFoundException(Type commandType, Type commandResult)
            : base(string.Format("Command handler not found for command type: {0}, and command result type: {1}", commandType, commandResult))
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion      
      
    }
}
