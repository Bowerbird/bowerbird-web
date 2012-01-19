/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.ViewModels.Members
{
    public class TeamUpdateInput : IViewModel
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required]
        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }

        #endregion

        #region Methods

        #endregion				
    }
}