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
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string Type { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string GroupType { get; set; }
            public int MemberCount { get; set; }
            public int ObservationCount { get; set; }
            public int ProjectCount { get; set; }
            public int TeamCount { get; set; }
            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Team Team { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
        }

        public All_Groups()
        {
            //AddMap<AppRoot>(appRoots => from appRoot in appRoots
            //                                      select new
            //                                      {
            //                                          Type = "group",
            //                                          appRoot.Id,
            //                                          appRoot.Name,
            //                                          GroupType = "approot",
            //                                          MemberCount = 0,
            //                                          ObservationCount = 0,
            //                                          ProjectCount = 0,
            //                                          TeamCount = 0
            //                                      });

            //AddMap<Organisation>(organisations => from organisation in organisations
            //                                      select new
            //                                                 {
            //                                                    Type = "group",
            //                                                    organisation.Id,
            //                                                    organisation.Name,
            //                                                    GroupType = "organisation",
            //                                                    MemberCount = 0,
            //                                                    ObservationCount = 0,
            //                                                    ProjectCount = 0,
            //                                                    TeamCount = 0
            //                                                 });

            //AddMap<Team>(teams => from team in teams
            //                                    select new
            //                                    {
            //                                        Type = "group",
            //                                        team.Id,
            //                                        team.Name,
            //                                        GroupType = "team",
            //                                        MemberCount = 0,
            //                                        ObservationCount = 0,
            //                                        ProjectCount = 0,
            //                                        TeamCount = 1
            //                                    });

            AddMap<Project>(projects => from project in projects
                                  select new
                                  {
                                      Type = "group",
                                      project.Id,
                                      project.Name,
                                      GroupType = "project",
                                      MemberCount = 0,
                                      ObservationCount = 0,
                                      ProjectCount = 1,
                                      TeamCount = 0
                                  });

            //AddMap<UserProject>(userProjects => from userProject in userProjects
            //                            select new
            //                            {
            //                                Type = "group",
            //                                userProject.Id,
            //                                userProject.Name,
            //                                GroupType = "userproject",
            //                                MemberCount = 0,
            //                                ObservationCount = 0,
            //                                ProjectCount = 0,
            //                                TeamCount = 0
            //                            });

            //AddMap<Member>(members => from member in members
            //                          select new
            //                          {
            //                              Type = "member",
            //                              member.Group.Id,
            //                              Name = (string)null,
            //                              GroupType = (string)null,
            //                              MemberCount = 1,
            //                              ObservationCount = 0,
            //                              ProjectCount = 0,
            //                              TeamCount = 0
            //                          });

            Reduce = results => from result in results
                                group result by result.Id
                                    into g
                                    select new
                                               {
                                                   Type = "result",
                                                   Id = g.Key,
                                                   g.Where(x => x.Name != null && x.GroupType != null && x.Type == "group").First().Name,
                                                   g.Where(x => x.Name != null && x.GroupType != null && x.Type == "group").First().GroupType,
                                                   MemberCount = 0,// g.Where(x => x.Type == "member").Sum(x => x.MemberCount),
                                                   ObservationCount = 0,
                                                   ProjectCount = 0,
                                                   TeamCount = 0
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
                    result.Id,
                    result.Type,
                    result.Name,
                    result.GroupType,
                    AppRoot = appRoot,
                    Organisation = organisation,
                    Team = team,
                    Project = project,
                    UserProject = userProject,
                    result.MemberCount,
                    ObservationCount = 0,
                    ProjectCount = 0,
                    TeamCount = 0
                };


            Store(x => x.Type, FieldStorage.Yes);
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.MemberCount, FieldStorage.Yes);
            Store(x => x.ObservationCount, FieldStorage.Yes);
            Store(x => x.ProjectCount, FieldStorage.Yes);
            Store(x => x.TeamCount, FieldStorage.Yes);
        }
    }
}