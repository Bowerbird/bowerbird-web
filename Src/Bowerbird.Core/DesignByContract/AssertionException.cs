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
    /// Exception raised when an assertion fails.
    /// </summary>
    public class AssertionException : DesignByContractException
    {
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException()
        {
        }

        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}