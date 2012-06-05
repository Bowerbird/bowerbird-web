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
    public class RequestPasswordResetEvent : DomainEventBase
    {

        #region Members

        #endregion

        #region Constructors

        public RequestPasswordResetEvent(
            User user,
            DomainModel sender)
            : base(
            user,
            sender)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion      
      
    }
}
