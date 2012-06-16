/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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

namespace Bowerbird.Web.ViewModels
{
    public class StreamInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string SearchQuery { get; set; }

        public DateTime? NewerThan { get; set; }

        public DateTime? OlderThan { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}