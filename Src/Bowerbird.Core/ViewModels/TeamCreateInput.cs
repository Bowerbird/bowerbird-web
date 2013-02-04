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

namespace Bowerbird.Core.ViewModels
{
    public class TeamCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string Website { get; set; }

        public string AvatarId { get; set; }

        public string OrganisationId { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}