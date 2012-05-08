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

namespace Bowerbird.Core.Indexes
{
    //public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    //{
    //    public class Result
    //    {
    //        public string Type { get; set; }
    //        public string Id { get; set; }
    //        public string ChildId { get; set; }
    //        public string Name { get; set; }
    //        public string GroupType { get; set; }
    //        public int MemberCount { get; set; }
    //        public int ProjectCount { get; set; }
    //        public int TeamCount { get; set; }
    //        public int OrganisationCount { get; set; }
    //        public int ChildGroupCount { get; set; }
    //        public AppRoot AppRoot { get; set; }
    //        public Organisation Organisation { get; set; }
    //        public Team Team { get; set; }
    //        public Project Project { get; set; }
    //        public UserProject UserProject { get; set; }
    //    }

    //    public All_Groups()
    //    {
    //        AddMap<AppRoot>(appRoots => from appRoot in appRoots
    //                                    select new
    //                                    {
    //                                        Type = "group",
    //                                        appRoot.Id,
    //                                        ChildId = (string)null,
    //                                        appRoot.Name,
    //                                        GroupType = "approot",
    //                                        MemberCount = 0,
    //                                        ProjectCount = 0,
    //                                        TeamCount = 0,
    //                                        OrganisationCount = 0,
    //                                        ChildGroupCount = 0
    //                                    });

    //        AddMap<Organisation>(organisations => from organisation in organisations
    //                                              select new
    //                                              {
    //                                                  Type = "group",
    //                                                  organisation.Id,
    //                                                  ChildId = (string)null,
    //                                                  organisation.Name,
    //                                                  GroupType = "organisation",
    //                                                  MemberCount = 0,
    //                                                  ProjectCount = 0,
    //                                                  TeamCount = 0,
    //                                                  OrganisationCount = 1,
    //                                                  ChildGroupCount = 0
    //                                              });

    //        AddMap<Team>(teams => from team in teams
    //                              select new
    //                              {
    //                                  Type = "group",
    //                                  team.Id,
    //                                  ChildId = (string)null,
    //                                  team.Name,
    //                                  GroupType = "team",
    //                                  MemberCount = 0,
    //                                  ProjectCount = 0,
    //                                  TeamCount = 1,
    //                                  OrganisationCount = 0,
    //                                  ChildGroupCount = 0
    //                              });

    //        AddMap<Project>(projects => from project in projects
    //                                    select new
    //                                    {
    //                                        Type = "group",
    //                                        project.Id,
    //                                        ChildId = (string)null,
    //                                        project.Name,
    //                                        GroupType = "project",
    //                                        MemberCount = 0,
    //                                        ProjectCount = 1,
    //                                        TeamCount = 0,
    //                                        OrganisationCount = 0,
    //                                        ChildGroupCount = 0
    //                                    });

    //        AddMap<UserProject>(userProjects => from userProject in userProjects
    //                                            select new
    //                                            {
    //                                                Type = "group",
    //                                                userProject.Id,
    //                                                ChildId = (string)null,
    //                                                userProject.Name,
    //                                                GroupType = "userproject",
    //                                                MemberCount = 0,
    //                                                ProjectCount = 0,
    //                                                TeamCount = 0,
    //                                                OrganisationCount = 0,
    //                                                ChildGroupCount = 0
    //                                            });

    //        AddMap<GroupAssociation>(groupAssociations => from groupAssociation in groupAssociations
    //                                                      select new
    //                                                      {
    //                                                          Type = "groupassociation",
    //                                                          Id = groupAssociation.ParentGroupId,
    //                                                          ChildId = groupAssociation.ChildGroupId,
    //                                                          Name = (string)null,
    //                                                          GroupType = "groupassociation",
    //                                                          MemberCount = 0,
    //                                                          ProjectCount = 0,
    //                                                          TeamCount = 0,
    //                                                          OrganisationCount = 0,
    //                                                          ChildGroupCount = 1
    //                                                      });

    //        AddMap<Member>(members => from member in members
    //                                  select new
    //                                  {
    //                                      Type = "member",
    //                                      member.Id,
    //                                      ChildId = (string)null,
    //                                      Name = (string)null,
    //                                      GroupType = "member",
    //                                      MemberCount = 1,
    //                                      ProjectCount = 0,
    //                                      TeamCount = 0,
    //                                      OrganisationCount = 0,
    //                                      ChildGroupCount = 0
    //                                  });

    //        Reduce = results => from result in results
    //                            group result by result.Id
    //                                into g
    //                                select new
    //                                {
    //                                    Type = g.Select(x => x.Type).FirstOrDefault(),
    //                                    Id = g.Key,
    //                                    ChildId = g.Select(x => x.ChildId).FirstOrDefault(),
    //                                    Name = g.Select(x => x.Name).FirstOrDefault(),
    //                                    GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
    //                                    MemberCount = g.Sum(x => x.MemberCount),
    //                                    ProjectCount = g.Where(x => x.GroupType == "project").Sum(x => x.ProjectCount),
    //                                    TeamCount = g.Where(x => x.GroupType == "team").Sum(x => x.TeamCount),
    //                                    OrganisationCount = g.Where(x => x.GroupType == "organisation").Sum(x => x.OrganisationCount),
    //                                    ChildGroupCount = g.Where(x => x.GroupType == "groupassociation").Sum(x => x.ChildGroupCount)
    //                                };

    //        TransformResults = (database, results) =>
    //            from result in results
    //            let appRoot = database.Load<AppRoot>(result.Id)
    //            let organisation = database.Load<Organisation>(result.Id)
    //            let team = database.Load<Team>(result.Id)
    //            let project = database.Load<Project>(result.Id)
    //            let userProject = database.Load<UserProject>(result.Id)
    //            select new
    //            {
    //                result.Id,
    //                result.Type,
    //                result.Name,
    //                result.GroupType,
    //                AppRoot = appRoot,
    //                Organisation = organisation,
    //                Team = team,
    //                Project = project,
    //                UserProject = userProject,
    //                result.MemberCount,
    //                result.ProjectCount,
    //                result.TeamCount,
    //                result.OrganisationCount,
    //                result.ChildGroupCount
    //            };


    //        Store(x => x.Type, FieldStorage.Yes);
    //        Store(x => x.Id, FieldStorage.Yes);
    //        Store(x => x.Name, FieldStorage.Yes);
    //        Store(x => x.GroupType, FieldStorage.Yes);
    //        Store(x => x.MemberCount, FieldStorage.Yes);
    //        Store(x => x.ProjectCount, FieldStorage.Yes);
    //        Store(x => x.TeamCount, FieldStorage.Yes);
    //        Store(x => x.OrganisationCount, FieldStorage.Yes);
    //        Store(x => x.ChildGroupCount, FieldStorage.Yes);
    //    }
    //}


    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string Type { get; set; }
            public string GroupType { get; set; }
            public string Id { get; set; }
            public int GroupMemberCount { get; set; }
            public int TeamProjectCount { get; set; }
            public int OrganisationTeamCount { get; set; }
            public int AppRootOrganisationCount { get; set; }
            public int AppRootUserProjectCount { get; set; }
            public int AppRootTeamCount { get; set; }
            public int AppRootProjectCount { get; set; }
            public Group Group { get { return AppRoot ?? Organisation ?? Team ?? Project ?? UserProject ?? (Group)null; } }
            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
        }

        public All_Groups()
        {
            AddMap<AppRoot>(appRoots => from appRoot in appRoots
                                        select new
                                        {
                                            Type = "group",
                                            GroupType = "approot",
                                            appRoot.Id,
                                            GroupMemberCount = 0,
                                            TeamProjectCount = 0,
                                            OrganisationTeamCount = 0,
                                            AppRootOrganisationCount = 0,
                                            AppRootUserProjectCount = 0,
                                            AppRootTeamCount = 0,
                                            AppRootProjectCount = 0
                                        });

            AddMap<Organisation>(organisations => from organisation in organisations
                                                  select new
                                                        {
                                                            Type = "group",
                                                            GroupType = "organisation",
                                                            organisation.Id,
                                                            GroupMemberCount = 0,
                                                            TeamProjectCount = 0,
                                                            OrganisationTeamCount = 0,
                                                            AppRootOrganisationCount = 0,
                                                            AppRootUserProjectCount = 0,
                                                            AppRootTeamCount = 0,
                                                            AppRootProjectCount = 0
                                                        });

            AddMap<Team>(teams => from team in teams
                                  select new
                                  {
                                      Type = "group",
                                      GroupType = "team",
                                      team.Id,
                                      GroupMemberCount = 0,
                                      TeamProjectCount = 0,
                                      OrganisationTeamCount = 0,
                                      AppRootOrganisationCount = 0,
                                      AppRootUserProjectCount = 0,
                                      AppRootTeamCount = 0,
                                      AppRootProjectCount = 0
                                  });

            AddMap<Project>(projects => from project in projects
                                        select new
                                        {
                                            Type = "group",
                                            GroupType = "project",
                                            project.Id,
                                            GroupMemberCount = 0,
                                            TeamProjectCount = 0,
                                            OrganisationTeamCount = 0,
                                            AppRootOrganisationCount = 0,
                                            AppRootUserProjectCount = 0,
                                            AppRootTeamCount = 0,
                                            AppRootProjectCount = 0
                                        });

            AddMap<UserProject>(userProjects => from userProject in userProjects
                                                select new
                                                {
                                                    Type = "group",
                                                    GroupType = "userproject",
                                                    userProject.Id,
                                                    GroupMemberCount = 0,
                                                    TeamProjectCount = 0,
                                                    OrganisationTeamCount = 0,
                                                    AppRootOrganisationCount = 0,
                                                    AppRootUserProjectCount = 0,
                                                    AppRootTeamCount = 0,
                                                    AppRootProjectCount = 0
                                                });

            AddMap<GroupAssociation>(groupAssociations => from groupAssociation in groupAssociations
                                                          select new
                                                          {
                                                              Type = "groupassociation",
                                                              GroupType = (string)null,
                                                              Id = groupAssociation.ParentGroupId,
                                                              GroupMemberCount = 0,
                                                              TeamProjectCount = groupAssociation.ParentGroupType == "team" && groupAssociation.ChildGroupType == "project" ? 1 : 0,
                                                              OrganisationTeamCount = groupAssociation.ParentGroupType == "organisation" && groupAssociation.ChildGroupType == "team" ? 1 : 0,
                                                              AppRootOrganisationCount = groupAssociation.ParentGroupType == "approot" && groupAssociation.ChildGroupType == "organisation" ? 1 : 0,
                                                              AppRootUserProjectCount = groupAssociation.ParentGroupType == "approot" && groupAssociation.ChildGroupType == "userproject" ? 1 : 0,
                                                              AppRootTeamCount = groupAssociation.ParentGroupType == "approot" && groupAssociation.ChildGroupType == "team" ? 1 : 0,
                                                              AppRootProjectCount = groupAssociation.ParentGroupType == "approot" && groupAssociation.ChildGroupType == "project" ? 1 : 0
                                                          });

            AddMap<Member>(members => from member in members
                                      select new
                                      {
                                          Type = "member",
                                          GroupType = (string)null,
                                          member.Group.Id,
                                          GroupMemberCount = 1,
                                          TeamProjectCount = 0,
                                          OrganisationTeamCount = 0,
                                          AppRootOrganisationCount = 0,
                                          AppRootUserProjectCount = 0,
                                          AppRootTeamCount = 0,
                                          AppRootProjectCount = 0
                                      });

            Reduce = results => from result in results
                                group result by result.Id
                                    into g
                                    select new
                                    {
                                        Type = g.Select(x => x.Type).FirstOrDefault(),
                                        GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
                                        Id = g.Key,
                                        GroupMemberCount = g.Sum(x => x.GroupMemberCount),
                                        TeamProjectCount = g.Sum(x => x.TeamProjectCount),
                                        OrganisationTeamCount = g.Sum(x => x.OrganisationTeamCount),
                                        AppRootOrganisationCount = g.Sum(x => x.AppRootOrganisationCount),
                                        AppRootUserProjectCount = g.Sum(x => x.AppRootUserProjectCount),
                                        AppRootTeamCount = g.Sum(x => x.AppRootTeamCount),
                                        AppRootProjectCount = g.Sum(x => x.AppRootProjectCount)
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
                    result.Type,
                    result.GroupType,
                    result.Id,
                    result.GroupMemberCount,
                    result.TeamProjectCount,
                    result.OrganisationTeamCount,
                    result.AppRootOrganisationCount,
                    result.AppRootUserProjectCount,
                    result.AppRootTeamCount,
                    result.AppRootProjectCount,
                    AppRoot = appRoot,
                    Organisation = organisation,
                    Team = team,
                    Project = project,
                    UserProject = userProject
                };

            Store(x => x.Type, FieldStorage.Yes);
            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.GroupMemberCount, FieldStorage.Yes);
            Store(x => x.TeamProjectCount, FieldStorage.Yes);
            Store(x => x.OrganisationTeamCount, FieldStorage.Yes);
            Store(x => x.AppRootOrganisationCount, FieldStorage.Yes);
            Store(x => x.AppRootUserProjectCount, FieldStorage.Yes);
            Store(x => x.AppRootTeamCount, FieldStorage.Yes);
            Store(x => x.AppRootProjectCount, FieldStorage.Yes);
        }
    }
}