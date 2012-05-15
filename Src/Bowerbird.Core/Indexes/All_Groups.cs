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
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using Raven.Client.Linq;

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string GroupType { get; set; }
            public string Id { get; set; }
            public int GroupMemberCount { get; set; }
            public string ParentGroupId { get; set; }
            public string[] ChildGroupIds { get; set; }
            public string[] AncestorGroupIds { get; set; }
            public string[] DescendantGroupIds { get; set; }
            public Group Group { get { return AppRoot ?? Organisation ?? Team ?? Project ?? UserProject ?? (Group)null; } }
            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }

        }

        public All_Groups()
        {
            AddMap<AppRoot>(
                appRoots => from appRoot in appRoots
                    select new
                    {
                        GroupType = "approot",
                        appRoot.Id,
                        GroupMemberCount = 0,
                        ParentGroupId = (string)null,
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = new string[] { },
                        DescendantGroupIds = new string[] { }
                    });

            AddMap<Organisation>(
                organisations => from organisation in organisations
                    select new
                    {
                        GroupType = "organisation",
                        organisation.Id,
                        GroupMemberCount = 0,
                        ParentGroupId = organisation.Ancestry.FirstOrDefault(),
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = organisation.Ancestry,
                        DescendantGroupIds = organisation.Descendants
                    });

            AddMap<Team>(
                teams => from team in teams
                    select new
                    {
                        GroupType = "team",
                        team.Id,
                        GroupMemberCount = 0,
                        ParentGroupId = 
                            team.Ancestry.Where(x => x.ToLower().Contains("organisations/")).FirstOrDefault()
                            ?? team.Ancestry.FirstOrDefault(),
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = team.Ancestry,
                        DescendantGroupIds = team.Descendants
                    });

            AddMap<Project>(
                projects => from project in projects
                    select new
                    {
                        GroupType = "project",
                        project.Id,
                        GroupMemberCount = 0,
                        ParentGroupId =
                            project.Ancestry.Where(x => x.ToLower().Contains("teams/")).FirstOrDefault()
                            ?? project.Ancestry.FirstOrDefault(),
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = project.Ancestry,
                        DescendantGroupIds = new string[] { }
                    });

            AddMap<UserProject>(
                userProjects => from userProject in userProjects
                    select new
                    {
                        GroupType = "userproject",
                        userProject.Id,
                        GroupMemberCount = 0,
                        ParentGroupId = userProject.Ancestry.FirstOrDefault(),
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = userProject.Ancestry,
                        DescendantGroupIds = new string[] { }
                    });

            AddMap<GroupAssociation>(
                groupAssociations => from groupAssociation in groupAssociations
                    select new
                    {
                        GroupType = "groupassociation",
                        Id = groupAssociation.ParentGroupId,
                        GroupMemberCount = 0,
                        ParentGroupId = (string)null,
                        ChildGroupIds = new[] { groupAssociation.ChildGroupId },
                        AncestorGroupIds = new string[] { },
                        DescendantGroupIds = new string[] { }
                    });

            AddMap<Member>(
                members => from member in members
                    select new
                    {
                        GroupType = (string)null,
                        member.Group.Id,
                        GroupMemberCount = 1,
                        ParentGroupId = (string)null,
                        ChildGroupIds = new string[] { },
                        AncestorGroupIds = new string[] { },
                        DescendantGroupIds = new string[] { }
                    });

            Reduce = results => from result in results
                group result by result.Id
                    into g
                    select new
                    {
                        GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
                        Id = g.Key,
                        GroupMemberCount = g.Sum(x => x.GroupMemberCount),
                        ParentGroupId = g.Select(x => x.ParentGroupId),
                        ChildGroupIds = g.SelectMany(x => x.ChildGroupIds),
                        AncestorGroupIds = g.SelectMany(x => x.AncestorGroupIds),
                        DescendantGroupIds = g.SelectMany(x => x.DescendantGroupIds)
                    };

            TransformResults = (database, results) =>
                from result in results
                let appRoot = database.Load<AppRoot>(result.Id)
                let organisation = database.Load<Organisation>(result.Id)
                let team = database.Load<Team>(result.Id)
                let project = database.Load<Project>(result.Id)
                let userProject = database.Load<UserProject>(result.Id)
                select new
                {
                    result.GroupType,
                    result.Id,
                    result.GroupMemberCount,
                    result.ParentGroupId,
                    result.ChildGroupIds,
                    result.AncestorGroupIds,
                    result.DescendantGroupIds,
                    AppRoot = appRoot,
                    Organisation = organisation,
                    Team = team,
                    Project = project,
                    UserProject = userProject
                };

            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.GroupMemberCount, FieldStorage.Yes);
            Store(x => x.ParentGroupId, FieldStorage.Yes);
            Store(x => x.ChildGroupIds, FieldStorage.Yes);
            Store(x => x.AncestorGroupIds, FieldStorage.Yes);
            Store(x => x.DescendantGroupIds, FieldStorage.Yes);
        }
    }
}