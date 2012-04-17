﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.ViewModels
{
    public class ProjectCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Team { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string Website { get; set; }

        // this is the mediaresourceid of the avatar mediaresrouce
        public string Avatar { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}