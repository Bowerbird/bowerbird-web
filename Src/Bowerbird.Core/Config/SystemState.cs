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

namespace Bowerbird.Core.Config
{
    public class SystemState
    {

        #region Fields

        #endregion

        #region Constructors

        public SystemState()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public DateTime? SystemDataSetupDate { get; set; }

        public bool FireEvents { get; set; }

        public bool SendEmails { get; set; }

        public bool ExecuteCommands { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Id = "settings/systemstate";
        }

        #endregion   
      
    }
}