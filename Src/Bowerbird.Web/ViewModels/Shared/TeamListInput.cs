/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels.Members
{
    public class TeamListInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string OrganisationId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public bool HasAddProjectPermission { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}