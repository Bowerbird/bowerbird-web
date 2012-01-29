/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.ViewModels.Members
{
    public class ProjectMemberList : IViewModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public Project Project { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public PagedList<ProjectMember> ProjectMembers { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}
