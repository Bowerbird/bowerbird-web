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

namespace Bowerbird.Web.ViewModels.Members
{
    public class ProjectPostUpdateInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        public ProjectPostUpdateInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public IList<string> MediaResources { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        #endregion
    }
}