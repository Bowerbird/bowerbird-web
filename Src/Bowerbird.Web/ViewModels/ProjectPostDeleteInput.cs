/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels
{
    public class ProjectPostDeleteInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        public ProjectPostDeleteInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public virtual string UserId { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        #endregion
    }
}