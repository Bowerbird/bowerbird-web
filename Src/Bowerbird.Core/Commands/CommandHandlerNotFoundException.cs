/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;

namespace Bowerbird.Core.Commands
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
