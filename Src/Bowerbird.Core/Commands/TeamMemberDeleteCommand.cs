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

namespace Bowerbird.Core.Commands
{
    public class TeamMemberDeleteCommand : ICommand
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string TeamId { get; set; }

        public string MemberId { get; set; }

        public string DeletedByUserId { get; set; }

        public string UserId { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}