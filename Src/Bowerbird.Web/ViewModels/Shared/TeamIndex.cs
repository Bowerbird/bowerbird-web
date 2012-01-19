/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class TeamIndex : IViewModel
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public Team Team { get; set; }

        public IList<Project> Projects { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}