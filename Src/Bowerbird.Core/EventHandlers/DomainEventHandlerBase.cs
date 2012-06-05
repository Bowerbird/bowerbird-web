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
using System.Collections.Generic;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.EventHandlers
{
    public abstract class DomainEventHandlerBase
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected Activity MakeActivity<T>(DomainModelCreatedEvent<T> domainEvent, string type, string description, IEnumerable<dynamic> groups) where T: DomainModel
        {
            return new Activity(
                type,
                DateTime.Now,
                description,
                domainEvent.User,
                groups);
        }

        #endregion
    }
}