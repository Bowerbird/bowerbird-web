/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;

namespace Bowerbird.Web.ViewModels.Members
{
    public class TeamProjectCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The Id of the Team the project is being added to
        /// </summary>
        public string ProjectTeamId { get; set; }

        public List<string> Administrators { get; set; }

        public List<string> Members { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
