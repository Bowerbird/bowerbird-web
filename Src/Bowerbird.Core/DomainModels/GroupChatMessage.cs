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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class GroupChatMessage : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Content { get; set; }
        
        public DenormalisedNamedDomainModelReference<Group> Group { get; set; }
        
        public DenormalisedUserReference User { get; set; }
        
        public DateTimeOffset When { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}