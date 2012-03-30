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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Group : DomainModel, IOwnable, INamedDomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Group()
        {
        }

        protected Group(
            User createdByUser,
            string name,
            DateTime createdDateTime)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            Name = name;
            User = createdByUser;
            CreatedDateTime = createdDateTime;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public DenormalisedUserReference User { get; protected set; } // User is protected set to allow AppRoot to set it after instantiation

        [JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return new string[] { this.Id }; }
        }

        #endregion

        #region Methods

        protected void SetDetails(string name)
        {
            Name = name;
        }

        #endregion      
    }
}