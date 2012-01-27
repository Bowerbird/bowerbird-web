/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
    public class OrganisationUpdateCommand : ICommand
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}