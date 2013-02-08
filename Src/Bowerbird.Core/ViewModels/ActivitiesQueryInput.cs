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

namespace Bowerbird.Core.ViewModels
{
    public class ActivitiesQueryInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Query { get; set; }

        public DateTime? NewerThan { get; set; }

        public DateTime? OlderThan { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}