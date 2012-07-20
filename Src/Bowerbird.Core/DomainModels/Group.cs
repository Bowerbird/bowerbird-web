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

namespace Bowerbird.Core.DomainModels
{
    public abstract class Group : DomainModel, IOwnable
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
            DateTime createdDateTime,
            Group parentGroup)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            Name = name;
            User = createdByUser;
            CreatedDateTime = createdDateTime;
            AddParentGroup(parentGroup);
        }

        #endregion

        #region Properties

        public string Name { get; protected set; } // Protected set to allow AppRoot to set it after instantiation

        public DateTime CreatedDateTime { get; protected set; } // Protected set to allow AppRoot to set it after instantiation

        public DenormalisedUserReference User { get; protected set; } // Protected set to allow AppRoot to set it after instantiation

        public DenormalisedGroupReference ParentGroup { get; private set; }

        public IEnumerable<DenormalisedGroupReference> AncestorGroups { get; private set; }

        public IEnumerable<DenormalisedGroupReference> ChildGroups { get; private set; }

        public IEnumerable<DenormalisedGroupReference> DescendantGroups { get; private set; }

        public abstract string GroupType { get; }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return new string[] { this.Id }; }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            ChildGroups = new List<DenormalisedGroupReference>();
            AncestorGroups = new List<DenormalisedGroupReference>();
            DescendantGroups = new List<DenormalisedGroupReference>();
        }

        protected void SetDetails(string name)
        {
            Name = name;
        }

        public Group AddParentGroup(Group group)
        {
            Check.RequireNotNull(group, "group");

            ParentGroup = group;
            AncestorGroups = group.AncestorGroups.Union(new DenormalisedGroupReference[] { group });
            return this;
        }

        public Group AddChildGroup(Group group)
        {
            Check.RequireNotNull(group, "group");

            ((List<DenormalisedGroupReference>)ChildGroups).Add(group);
            ((List<DenormalisedGroupReference>)DescendantGroups).Add(group);
            return this; 
        }

        public Group AddDescendantGroup(Group group)
        {
            Check.RequireNotNull(group, "group");

            ((List<DenormalisedGroupReference>)DescendantGroups).Add(group);
            return this;
        }

        public Group RemoveDescendantGroup(Group group)
        {
            Check.RequireNotNull(group, "group");

            ((List<DenormalisedGroupReference>)DescendantGroups).RemoveAll(x => x.Id == group.Id);
            return this;
        }

        #endregion
    }
}