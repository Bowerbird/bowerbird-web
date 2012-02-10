/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels.Shared
{
    public class StreamSortInput
    {
        #region Fields

        #endregion

        #region Constructors

        public StreamSortInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string GroupId { get; set; }

        public bool DateTimeDescending { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            DateTimeDescending = true;
        }

        #endregion				
    }
}