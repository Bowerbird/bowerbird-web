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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.ViewModels
{
    public class PostUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        public PostUpdateInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        [Required]
        public string Id { get; set; }

        [Required]
        public string GroupId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string PostType { get; set; }

        public IList<string> MediaResources { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            MediaResources = new List<string>();
        }

        #endregion
    }
}