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

namespace Bowerbird.Web.ViewModels
{
    public class PostCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        public PostCreateInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        [Required]
        public string GroupId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public IList<string> MediaResources { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        #endregion

        #region Methods

        public void InitMembers()
        {
            MediaResources = new List<string>();
        }

        #endregion
    }
}