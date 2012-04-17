/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.ViewModels
{
    public class ObservationIndex : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        public ObservationIndex()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public Observation Observation { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        #endregion
    }
}