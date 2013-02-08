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
using Bowerbird.Core.Internationalisation;

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

        public string Id { get; set; }

        public string Key { get; set; }

        public string GroupId { get; set; }

        public string GroupType { get; set; }

        [Required(ErrorMessageResourceName = "TitleRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceName = "PostTypeRequired", ErrorMessageResourceType = typeof(I18n))]
        public string PostType { get; set; }

        [Required(ErrorMessageResourceName = "MessageRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Message { get; set; }

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