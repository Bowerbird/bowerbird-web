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

namespace Bowerbird.Web.ViewModels.Shared
{
    public class StreamItemViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Type { get; set; }

        public DateTime SubmittedOn { get; set; }

        public object Item { get; set; }

        public string ParentId { get; set; }

        public string ItemId { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}