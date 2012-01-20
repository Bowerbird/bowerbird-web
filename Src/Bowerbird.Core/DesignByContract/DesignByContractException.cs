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

namespace Bowerbird.Core.DesignByContract
{
    /// <summary>
    ///     Exception raised when a contract is broken.
    ///     Catch this exception type if you wish to differentiate between 
    ///     any DesignByContract exception and other runtime exceptions.
    /// </summary>
    public class DesignByContractException : ApplicationException
    {
        protected DesignByContractException()
        {
        }

        protected DesignByContractException(string message)
            : base(message)
        {
        }

        protected DesignByContractException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}