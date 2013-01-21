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
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Favourites : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected Favourites()
            : base()
        {
        }

        public Favourites(
            User createdByUser,
            DateTime createdDateTime,
            Group parentGroup)
            : base(
            createdByUser,
            "Favourites",
            createdDateTime,
            parentGroup)
        {
            ApplyEvent(new DomainModelCreatedEvent<Favourites>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "favourites"; }
        }
        
        #endregion

        #region Methods

        #endregion
    }
}