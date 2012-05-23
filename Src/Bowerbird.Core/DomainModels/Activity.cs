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
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Activity : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Activity()
        {
        }

        public Activity(
            string type,
            DateTime createdDateTime, 
            string createdByUser,
            object content,
            object subContent)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            Type = type;
            CreatedDateTime = createdDateTime;
            UserId = createdByUser;
            Content = content;
            SubContent = subContent;
        }

        #endregion

        #region Properties

        public string Type { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public string UserId { get; private set; }

        public object Content { get; private set; }

        public object SubContent { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}