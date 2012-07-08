/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using Raven.Client.Linq;
using System.Collections.Generic;
using System.Collections;

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

            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
            public IEnumerable<User> Users { get; set; }

            public Group Group
            {
                get
                {
                    switch (GroupType)
                    {
                        case "userproject":
                            return UserProject;
                        case "project":
                            return Project;
                        case "team":
                            return Team;
                        case "organisation":
                            return Organisation;
                        case "approot":
                            return AppRoot;
                        default:
                            return null;
                    }
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
                                GroupRoleIds = new string[] { }
                            });

            AddMap<Organisation>(
                organisations =>
                    from organisation in organisations
                    let parentGroup = organisation.Ancestry.FirstOrDefault()
                    select new
                    {
                        organisation.GroupType,
                        GroupId = organisation.Id,
                        UserIds = new string[] { },
                        ParentGroupId = parentGroup.Id,
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = from ancestor in organisation.Ancestry
                                           select ancestor.Id,
                        DescendantGroupIds = from descendant in organisation.Descendants
                                             select descendant.Id,
                        GroupRoleIds = new string[] { }                                             
                    });

            AddMap<Team>(
                teams =>
                    from team in teams
                    let parentGroup = team.Ancestry.Where(x => x.GroupType == "organisation").FirstOrDefault() ?? team.Ancestry.FirstOrDefault()
                    select new
                    {
                        team.GroupType,
                        GroupId = team.Id,
                        UserIds = new string[] { },
                        ParentGroupId = parentGroup.Id,
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = from ancestor in team.Ancestry
                                           select ancestor.Id,
                        DescendantGroupIds = from descendant in team.Descendants
                                             select descendant.Id,
                        GroupRoleIds = new string[] { }
                    });

            AddMap<Project>(
                projects => from project in projects
                            let parentGroup = project.Ancestry.Where(x => x.GroupType == "team").FirstOrDefault() ?? project.Ancestry.FirstOrDefault()
                            select new
                            {
                                project.GroupType,
                                GroupId = project.Id,
                                UserIds = new string[] { },
                                ParentGroupId = parentGroup.Id,
                                ChildGroupIds = new string[] { },
                                AncestorGroupIds = from ancestor in project.Ancestry
                                                   select ancestor.Id,
                                DescendantGroupIds = new string[] { },
                                GroupRoleIds = new string[] { }
                            });

            AddMap<UserProject>(
                userProjects => from userProject in userProjects
                                let parentGroup = userProject.Ancestry.FirstOrDefault()
                                select new
                                {
                                    userProject.GroupType,
                                    GroupId = userProject.Id,
                                    UserIds = new string[] { },
                                    ParentGroupId = parentGroup.Id,
                                    ChildGroupIds = new string[] { },
                                    AncestorGroupIds = from ancestor in userProject.Ancestry
                                                       select ancestor.Id,
                                    DescendantGroupIds = new string[] { },
                                    GroupRoleIds = new string[] { }
                                });

            AddMap<GroupAssociation>(
                groupAssociations => from groupAssociation in groupAssociations
                                     select new
                                     {
                                         groupAssociation.ParentGroup.GroupType,
                                         GroupId = groupAssociation.ParentGroup.Id,
                                         UserIds = new string[] { },
                                         ParentGroupId = (string)null,
                                         ChildGroupIds = new[] { groupAssociation.ChildGroup.Id },
                                         AncestorGroupIds = new string[] { },
                                         DescendantGroupIds = new string[] { },
                                         GroupRoleIds = new string[] { }
                                     });

            AddMap<User>(
                users => from user in users
                         from member in user.Memberships
                         select new
                         {
                             member.Group.GroupType,
                             GroupId = member.Group.Id,
                             UserIds = new string[] {user.Id },
                             ParentGroupId = (string)null,
                             ChildGroupIds = new string[] { },
                             AncestorGroupIds = new string[] { },
                             DescendantGroupIds = new string[] { },
                             GroupRoleIds = member.Roles.Select(x => x.Id)
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
                                        GroupRoleIds = g.SelectMany(x => x.GroupRoleIds)
                                    };

            TransformResults = (database, results) =>
                from result in results
                let appRoot = database.Load<AppRoot>(result.GroupId)
                let organisation = database.Load<Organisation>(result.GroupId)
                let team = database.Load<Team>(result.GroupId)
                let project = database.Load<Project>(result.GroupId)
                let userProject = database.Load<UserProject>(result.GroupId)
                let users = database.Load<User>(result.UserIds)
                select new
                {
                    result.GroupType,
                    result.GroupId,
                    result.UserIds,
                    result.ParentGroupId,
                    result.ChildGroupIds,
                    result.AncestorGroupIds,
                    result.DescendantGroupIds,
                    result.GroupRoleIds,
                    AppRoot = result.GroupType == "approot" ? appRoot : null,
                    Organisation = result.GroupType == "organisation" ? organisation : null,
                    Team = result.GroupType == "team" ? team : null,
                    Project = result.GroupType == "project" ? project : null,
                    UserProject = result.GroupType == "userproject" ? userProject : null,
                    Users = users
                };

            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.UserIds, FieldStorage.Yes);
            Store(x => x.ParentGroupId, FieldStorage.Yes);
            Store(x => x.ChildGroupIds, FieldStorage.Yes);
            Store(x => x.AncestorGroupIds, FieldStorage.Yes);
            Store(x => x.DescendantGroupIds, FieldStorage.Yes);
            Store(x => x.GroupRoleIds, FieldStorage.Yes);
        }
    }
}