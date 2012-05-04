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

using Bowerbird.Core.Config;
using System;

namespace Bowerbird.Core.DomainModels
{
    public class AppRoot : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected AppRoot() : base() { }

        public AppRoot(
            User createdByUser)
            : base(
            createdByUser,
            "Application Root Group",
            DateTime.Now)
        {
            Id = Constants.AppRootId;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}