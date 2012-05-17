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
    public abstract class Group : DomainModel, IOwnable, INamedDomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Group()
        {
            InitMembers();
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

        public string Name { get; protected set; } // Protected set to allow AppRoot to set it after instantiation

        public DateTime CreatedDateTime { get; private set; }

        public DenormalisedUserReference User { get; protected set; } // Protected set to allow AppRoot to set it after instantiation

        public IEnumerable<string> Ancestry { get; private set; }

        public IEnumerable<string> Descendants { get; private set; }

        public abstract string GroupType { get; }

        [JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return new string[] { this.Id }; }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Ancestry = new List<string>();
            Descendants = new List<string>();
        }

        protected void SetDetails(string name)
        {
            Name = name;
        }

        public Group SetAncestry(Group groupContainingAncestry)
        {
            Ancestry = groupContainingAncestry.Ancestry.Union(new [] { groupContainingAncestry.Id });

            return this;
        }

        public Group AddDescendant(Group group)
        {
            ((List<string>)Descendants).Add(group.Id);
            return this; 
        }

        public Group RemoveDescendant(Group group)
        {
            ((List<string>)Descendants).Remove(group.Id);
            return this;
        }

        #endregion
    }
}