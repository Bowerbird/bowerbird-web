﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Group : DomainModel, INamedDomainModel
    {
        #region Members

        private List<GroupAssociation> _childGroupAssociations;

        #endregion

        #region Constructors

        protected Group()
        {
            InitMembers();
        }

        protected Group(
            string name)
            : this()
        {
            Check.RequireNotNullOrWhitespace(name, "name");

            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string ParentGroupId { get; protected set; }

        public string Description { get; private set; }

        public string Website { get; private set; }

        public string AvatarId { get; set; }

        public IEnumerable<GroupAssociation> ChildGroupAssociations { get { return _childGroupAssociations; } }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _childGroupAssociations = new List<GroupAssociation>();
        }

        protected void SetDetails(string name, string description, string website, string avatarId, string parentGroupId = null)
        {
            Name = name;
            Description = description;
            Website = website;
            AvatarId = avatarId;
            ParentGroupId = parentGroupId;
        }
        
        public void AddGroupAssociation(Group group, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            if (_childGroupAssociations.All(x => x.GroupId != group.Id))
            {
                var groupAssociation = new GroupAssociation(group, createdByUser, createdDateTime);

                _childGroupAssociations.Add(groupAssociation);

                EventProcessor.Raise(new DomainModelCreatedEvent<GroupAssociation>(groupAssociation, createdByUser));
            }
        }

        public void RemoveGroupAssociation(string groupId)
        {
            if (_childGroupAssociations.Any(x => x.GroupId == groupId))
            {
                _childGroupAssociations.RemoveAll(x => x.GroupId == groupId);
            }
        }


        #endregion      
    }
}