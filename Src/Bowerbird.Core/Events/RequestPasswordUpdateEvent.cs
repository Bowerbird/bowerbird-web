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
    public class RequestPasswordUpdate : DomainEventBase
    {

        #region Members

        #endregion

        #region Constructors

        public RequestPasswordUpdate(
            User user,
            DomainModel sender,
            bool sendEmail)
            : base(
            user,
            sender)
        {
            SendEmail = sendEmail;
        }

        #endregion

        #region Properties

        public bool SendEmail { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
