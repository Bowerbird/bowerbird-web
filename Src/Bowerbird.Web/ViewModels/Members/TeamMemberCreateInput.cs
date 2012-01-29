/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Team Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.ViewModels.Members
{
    public class TeamMemberCreateInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required]
        public string UserId { get; set; }

        [Required]
        public string TeamId { get; set; }

        [Required]
        public IEnumerable<string> Roles { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}