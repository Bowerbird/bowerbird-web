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

using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class RequestPasswordResetEvent : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public User User { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
