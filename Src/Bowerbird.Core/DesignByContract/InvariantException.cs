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
    ///     Exception raised when an invariant fails.
    /// </summary>
    public class InvariantException : DesignByContractException
    {
        /// <summary>
        ///     Invariant Exception.
        /// </summary>
        public InvariantException()
        {
        }

        /// <summary>
        ///     Invariant Exception.
        /// </summary>
        public InvariantException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Invariant Exception.
        /// </summary>
        public InvariantException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}