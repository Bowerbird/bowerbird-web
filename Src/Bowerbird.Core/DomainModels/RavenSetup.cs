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

namespace Bowerbird.Core.DomainModels
{
    public class RavenSetup : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        public RavenSetup()
        {
            SetupDate = DateTime.UtcNow;
        }

        #endregion

        #region Properties

        public DateTime SetupDate { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}