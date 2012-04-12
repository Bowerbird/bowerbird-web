/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Web.ViewModels.Shared;
using System.Collections.Generic;

namespace Bowerbird.Web.ViewModels.Public
{
    public class BrowseItem
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string type { get; set; }

        public DateTime createdDateTime { get; set; }

        public string createdDateTimeDescription { get; set; }

        public UserProfile user { get; set; }

        public object item { get; set; }

        public string description { get; set; }

        public IEnumerable<string> groups { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}