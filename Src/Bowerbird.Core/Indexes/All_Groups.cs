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

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string GroupType { get; set; }
            public string GroupId { get; set; }
            public object[] Memberships { get; set; }
            public object ParentGroup { get; set; }
            public object[] ChildGroups { get; set; }
            public object[] AncestorGroups { get; set; }
            public object[] DescendantGroups { get; set; }
            public Group Group { get { return AppRoot ?? Organisation ?? Team ?? Project ?? UserProject ?? (Group)null; } }
            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
        }

        public class ClientResult
        {
            public string GroupType { get; set; }
            public string GroupId { get; set; }
            public Membership[] Memberships { get; set; }
            public DenormalisedGroupReference ParentGroup { get; set; }
            public DenormalisedGroupReference[] ChildGroups { get; set; }
            public DenormalisedGroupReference[] AncestorGroups { get; set; }
            public DenormalisedGroupReference[] DescendantGroups { get; set; }
            public Group Group { get { return AppRoot ?? Organisation ?? Team ?? Project ?? UserProject ?? (Group)null; } }
            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }

        }

        public class Membership
        {
            public string UserId { get; set; }
            public Role[] Roles { get; set; }
        }

        public All_Groups()
        {
            AddMap<AppRoot>(
                appRoots => from appRoot in appRoots
                            select new
                            {
                                appRoot.GroupType,
                                GroupId = appRoot.Id,
                                Memberships = new object[] { },
                                ParentGroup = (object)null,
                                ChildGroups = new object[] { },
                                AncestorGroups = new object[] { },
                                DescendantGroups = new object[] { }
                            });

            AddMap<Organisation>(
                organisations =>
                    from organisation in organisations
                    let parentGroup = organisation.Ancestry.FirstOrDefault()
                    select new
                    {
                        organisation.GroupType,
                        GroupId = organisation.Id,
                        Memberships = new object[] { },
                        ParentGroup = new
                            {
                                parentGroup.Id,
                                parentGroup.GroupType
                            },
                        ChildGroups = new object[] { },
                        AncestorGroups = from ancestor in organisation.Ancestry
                                           select new
                                            {
                                                ancestor.Id,
                                                ancestor.GroupType
                                            },
                        DescendantGroups = from descendant in organisation.Descendants
                                           select new
                                            {
                                                descendant.Id,
                                                descendant.GroupType
                                            }
                    });

            AddMap<Team>(
                teams =>
                    from team in teams
                    let parentGroup = team.Ancestry.Where(x => x.GroupType == "organisation").FirstOrDefault() ?? team.Ancestry.FirstOrDefault()
                    select new
                    {
                        team.GroupType,
                        GroupId = team.Id,
                        Memberships = new object[] { },
                        ParentGroup = new
                            {
                                parentGroup.Id,
                                parentGroup.GroupType
                            },
                        ChildGroups = new object[] { },
                        AncestorGroups = from ancestor in team.Ancestry
                                         select new
                                         {
                                             ancestor.Id,
                                             ancestor.GroupType
                                         },
                        DescendantGroups = from descendant in team.Descendants
                                           select new
                                           {
                                               descendant.Id,
                                               descendant.GroupType
                                           }
                    });

            AddMap<Project>(
                projects => from project in projects
                            let parentGroup = project.Ancestry.Where(x => x.GroupType == "team").FirstOrDefault() ?? project.Ancestry.FirstOrDefault()
                            select new
                            {
                                project.GroupType,
                                GroupId = project.Id,
                                Memberships = new object[] { },
                                ParentGroup = new
                                {
                                    parentGroup.Id,
                                    parentGroup.GroupType
                                },
                                ChildGroups = new object[] { },
                                AncestorGroups = from ancestor in project.Ancestry
                                                 select new
                                                 {
                                                     ancestor.Id,
                                                     ancestor.GroupType
                                                 },
                                DescendantGroups = new object[] { }
                            });

            AddMap<UserProject>(
                userProjects => from userProject in userProjects
                                let parentGroup = userProject.Ancestry.FirstOrDefault()
                                select new
                                {
                                    userProject.GroupType,
                                    GroupId = userProject.Id,
                                    Memberships = new object[] { },
                                    ParentGroup = new
                                    {
                                        parentGroup.Id,
                                        parentGroup.GroupType
                                    },
                                    ChildGroups = new object[] { },
                                    AncestorGroups = from ancestor in userProject.Ancestry
                                                     select new
                                                     {
                                                         ancestor.Id,
                                                         ancestor.GroupType
                                                     },
                                    DescendantGroups = new object[] { }
                                });

            AddMap<GroupAssociation>(
                groupAssociations => from groupAssociation in groupAssociations
                                     select new
                                     {
                                         groupAssociation.ParentGroup.GroupType,
                                         GroupId = groupAssociation.ParentGroup.Id,
                                         Memberships = new object[] { },
                                         ParentGroup = (object)null,
                                         ChildGroups = new[]
                                                           {
                                                               new 
                                                               {
                                                                  groupAssociation.ChildGroup.Id,
                                                                  groupAssociation.ChildGroup.GroupType
                                                               }
                                                           },
                                         AncestorGroups = new object[] { },
                                         DescendantGroups = new object[] { }
                                     });

            AddMap<Member>(
                members => from member in members
                           select new
                           {
                               member.Group.GroupType,
                               GroupId = member.Group.Id,
                               Memberships = new object[] 
                                { 
                                    new
                                        {
                                            UserId = member.User.Id, 
                                            Roles = from role in member.Roles
                                                    select new
                                                    {
                                                        role.Id,
                                                        role.Name,
                                                        role.Description
                                                    }
                                        } 
                                },
                               ParentGroup = (object)null,
                               ChildGroups = new object[] { },
                               AncestorGroups = new object[] { },
                               DescendantGroups = new object[] { }
                           });

            Reduce = results => from result in results
                                group result by result.GroupId
                                    into g
                                    select new
                                    {
                                        GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
                                        GroupId = g.Key,
                                        Memberships = g.SelectMany(x => x.Memberships),
                                        ParentGroup = g.Select(x => x.ParentGroup),
                                        ChildGroups = g.SelectMany(x => x.ChildGroups),
                                        AncestorGroups = g.SelectMany(x => x.AncestorGroups),
                                        DescendantGroups = g.SelectMany(x => x.DescendantGroups)
                                    };

            TransformResults = (database, results) =>
                from result in results
                let appRoot = database.Load<AppRoot>(result.GroupId)
                let organisation = database.Load<Organisation>(result.GroupId)
                let team = database.Load<Team>(result.GroupId)
                let project = database.Load<Project>(result.GroupId)
                let userProject = database.Load<UserProject>(result.GroupId)
                select new
                {
                    result.GroupType,
                    result.GroupId,
                    result.Memberships,
                    result.ParentGroup,
                    result.ChildGroups,
                    result.AncestorGroups,
                    result.DescendantGroups,
                    AppRoot = appRoot,
                    Organisation = organisation,
                    Team = team,
                    Project = project,
                    UserProject = userProject
                };

            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.Memberships, FieldStorage.Yes);
            Store(x => x.ParentGroup, FieldStorage.Yes);
            Store(x => x.ChildGroups, FieldStorage.Yes);
            Store(x => x.AncestorGroups, FieldStorage.Yes);
            Store(x => x.DescendantGroups, FieldStorage.Yes);
        }
    }
}