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
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using System.Collections.Generic;

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string GroupType { get; set; }
            public string GroupId { get; set; }
            public string[] UserIds { get; set; }
            public string ParentGroupId { get; set; }
            public string[] ChildGroupIds { get; set; }
            public string[] AncestorGroupIds { get; set; }
            public string[] DescendantGroupIds { get; set; }
            public string[] GroupRoleIds { get; set; }
            public string Name { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public int UserCount { get; set; }
            public int SightingCount { get; set; }
            public int PostCount { get; set; }
            public int VoteCount { get; set; }

            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
            public Favourites Favourites { get; set; }

            public Group Group
            {
                get
                {
                    return AppRoot as Group ??
                           UserProject as Group ??
                           Favourites as Group ??
                           Project as Group ??
                           Organisation as Group;
                }
            }
        }


        public All_Groups()  
        {
            AddMap<AppRoot>(
                appRoots => from appRoot in appRoots
                            select new
                            {
                                appRoot.GroupType,
                                GroupId = appRoot.Id,
                                UserIds = new string[] { },
                                ParentGroupId = (string)null,
                                ChildGroupIds = new string[] { },
                                AncestorGroupIds = new string[] { },
                                DescendantGroupIds = new string[] { },
                                GroupRoleIds = new string[] { },
                                appRoot.Name,
                                appRoot.CreatedDateTime,
                                UserCount = 0,
                                SightingCount = 0,
                                PostCount = 0,
                                VoteCount = 0
                            });

            AddMap<Organisation>(
                organisations =>
                    from organisation in organisations
                    let parentGroup = organisation.AncestorGroups.FirstOrDefault()
                    select new
                    {
                        organisation.GroupType,
                        GroupId = organisation.Id,
                        UserIds = new string[] { },
                        ParentGroupId = parentGroup.Id,
                        ChildGroupIds = from child in organisation.ChildGroups
                                        select child.Id,
                        AncestorGroupIds = from ancestor in organisation.AncestorGroups
                                           select ancestor.Id,
                        DescendantGroupIds = from descendant in organisation.DescendantGroups
                                             select descendant.Id,
                        GroupRoleIds = new string[] { },
                        organisation.Name,
                        organisation.CreatedDateTime,
                        UserCount = 0,
                        SightingCount = 0,
                        PostCount = 0,
                        VoteCount = 0
                    });

            AddMap<Project>(
                projects => from project in projects
                            let parentGroup = project.AncestorGroups.FirstOrDefault()
                            select new
                            {
                                project.GroupType,
                                GroupId = project.Id,
                                UserIds = new string[] { },
                                ParentGroupId = parentGroup.Id,
                                ChildGroupIds = new string[] { },
                                AncestorGroupIds = from ancestor in project.AncestorGroups
                                                   select ancestor.Id,
                                DescendantGroupIds = new string[] { },
                                GroupRoleIds = new string[] { },
                                project.Name,
                                project.CreatedDateTime,
                                UserCount = 0,
                                SightingCount = 0,
                                PostCount = 0,
                                VoteCount = 0
                            });

            AddMap<UserProject>(
                userProjects => from userProject in userProjects
                                let parentGroup = userProject.AncestorGroups.FirstOrDefault()
                                select new
                                {
                                    userProject.GroupType,
                                    GroupId = userProject.Id,
                                    UserIds = new string[] { },
                                    ParentGroupId = parentGroup.Id,
                                    ChildGroupIds = new string[] { },
                                    AncestorGroupIds = from ancestor in userProject.AncestorGroups
                                                       select ancestor.Id,
                                    DescendantGroupIds = new string[] { },
                                    GroupRoleIds = new string[] { },
                                    userProject.Name,
                                    userProject.CreatedDateTime,
                                    UserCount = 0,
                                    SightingCount = 0,
                                    PostCount = 0,
                                    VoteCount = 0
                                });

            AddMap<Favourites>(
                favouritesGroups => from favourites in favouritesGroups
                                    let parentGroup = favourites.AncestorGroups.FirstOrDefault()
                                    select new
                                        {
                                            favourites.GroupType,
                                            GroupId = favourites.Id,
                                            UserIds = new string[] {},
                                            ParentGroupId = parentGroup.Id,
                                            ChildGroupIds = new string[] {},
                                            AncestorGroupIds = from ancestor in favourites.AncestorGroups
                                                               select ancestor.Id,
                                            DescendantGroupIds = new string[] {},
                                            GroupRoleIds = new string[] {},
                                            favourites.Name,
                                            favourites.CreatedDateTime,
                                            UserCount = 0,
                                            SightingCount = 0,
                                            PostCount = 0,
                                            VoteCount = 0
                                        });

            // Memberships of groups
            AddMap<User>(
                users => from user in users
                         from member in user.Memberships
                         select new
                         {
                             member.Group.GroupType,
                             GroupId = member.Group.Id,
                             UserIds = new [] {user.Id },
                             ParentGroupId = (string)null,
                             ChildGroupIds = new string[] { },
                             AncestorGroupIds = new string[] { },
                             DescendantGroupIds = new string[] { },
                             GroupRoleIds = member.Roles.Select(x => x.Id),
                             Name = (string)null,
                             CreatedDateTime = (object)null,
                             UserCount = 1,
                             SightingCount = 0,
                             PostCount = 0,
                             VoteCount = 0
                         });

            // Count of observation sightings in groups
            AddMap<Observation>(
                observations => from observation in observations
                                from sightingGroup in observation.Groups
                                select new
                                    {
                                        sightingGroup.Group.GroupType,
                                        GroupId = sightingGroup.Group.Id,
                                        UserIds = new string[] {},
                                        ParentGroupId = (string) null,
                                        ChildGroupIds = new string[] {},
                                        AncestorGroupIds = new string[] {},
                                        DescendantGroupIds = new string[] {},
                                        GroupRoleIds = new string[] {},
                                        Name = (string) null,
                                        CreatedDateTime = (object) null,
                                        UserCount = 0,
                                        SightingCount = 1,
                                        PostCount = 0,
                                        VoteCount = 0
                                    });

            // Count of record sightings in groups
            AddMap<Record>(
                records => from record in records
                           from sightingGroup in record.Groups
                           select new
                               {
                                   sightingGroup.Group.GroupType,
                                   GroupId = sightingGroup.Group.Id,
                                   UserIds = new string[] {},
                                   ParentGroupId = (string) null,
                                   ChildGroupIds = new string[] {},
                                   AncestorGroupIds = new string[] {},
                                   DescendantGroupIds = new string[] {},
                                   GroupRoleIds = new string[] {},
                                   Name = (string) null,
                                   CreatedDateTime = (object) null,
                                   UserCount = 0,
                                   SightingCount = 1,
                                   PostCount = 0,
                                   VoteCount = 0
                               });

            // Count of posts in groups
            AddMap<Post>(
                posts => from post in posts
                         select new
                         {
                             post.Group.GroupType,
                             GroupId = post.Group.Id,
                             UserIds = new string[] { },
                             ParentGroupId = (string)null,
                             ChildGroupIds = new string[] { },
                             AncestorGroupIds = new string[] { },
                             DescendantGroupIds = new string[] { },
                             GroupRoleIds = new string[] { },
                             Name = (string)null,
                             CreatedDateTime = (object)null,
                             UserCount = 0,
                             SightingCount = 0,
                             PostCount = 1,
                             VoteCount = 0
                         });

            Reduce = results => from result in results
                                group result by result.GroupId
                                    into g
                                    select new
                                    {
                                        GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
                                        GroupId = g.Key,
                                        UserIds = g.SelectMany(x => x.UserIds),
                                        ParentGroupId = g.Select(x => x.ParentGroupId).Where(x => x != null).FirstOrDefault(),
                                        ChildGroupIds = g.SelectMany(x => x.ChildGroupIds),
                                        AncestorGroupIds = g.SelectMany(x => x.AncestorGroupIds),
                                        DescendantGroupIds = g.SelectMany(x => x.DescendantGroupIds),
                                        GroupRoleIds = g.SelectMany(x => x.GroupRoleIds),
                                        Name = g.Select(x => x.Name).Where(x => x != null).FirstOrDefault(),
                                        CreatedDateTime = g.Select(x => x.CreatedDateTime).Where(x => x != null).FirstOrDefault(),
                                        UserCount = g.Sum(x => x.UserCount),
                                        SightingCount = g.Sum(x => x.SightingCount),
                                        PostCount = g.Sum(x => x.PostCount),
                                        VoteCount = g.Sum(x => x.VoteCount)
                                    };

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.GroupType,
                    result.GroupId,
                    UserIds = result.UserIds ?? new string[]{},
                    result.ParentGroupId,
                    ChildGroupIds = result.ChildGroupIds ?? new string[] { },
                    AncestorGroupIds = result.AncestorGroupIds ?? new string[]{},
                    DescendantGroupIds = result.DescendantGroupIds ?? new string[]{},
                    GroupRoleIds = result.GroupRoleIds ?? new string[] { },
                    result.UserCount,
                    result.SightingCount,
                    result.PostCount,
                    result.VoteCount,
                    AppRoot = result.GroupType == "approot" ? database.Load<AppRoot>(result.GroupId) : null,
                    Organisation = result.GroupType == "organisation" ? database.Load<Organisation>(result.GroupId) : null,
                    Project = result.GroupType == "project" ? database.Load<Project>(result.GroupId) : null,
                    UserProject = result.GroupType == "userproject" ? database.Load<UserProject>(result.GroupId) : null,
                    Favourites = result.GroupType == "favourites" ? database.Load<Favourites>(result.GroupId) : null,
                };

            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.UserIds, FieldStorage.Yes);
            Store(x => x.ParentGroupId, FieldStorage.Yes);
            Store(x => x.ChildGroupIds, FieldStorage.Yes);
            Store(x => x.AncestorGroupIds, FieldStorage.Yes);
            Store(x => x.DescendantGroupIds, FieldStorage.Yes);
            Store(x => x.GroupRoleIds, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.UserCount, FieldStorage.Yes);
            Store(x => x.SightingCount, FieldStorage.Yes);
            Store(x => x.PostCount, FieldStorage.Yes);
            Store(x => x.VoteCount, FieldStorage.Yes);
        }
    }
}