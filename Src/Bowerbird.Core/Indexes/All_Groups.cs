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
            public string Id { get; set; }
            public string Name { get; set; }
            public string GroupType { get; set; }
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
                                                      appRoot.Id,
                                                      appRoot.Name,
                                                      GroupType = "approot"
                                                  });

            AddMap<Organisation>(organisations => from organisation in organisations
                                                  select new
                                                             { 
                                                                organisation.Id,
                                                                organisation.Name,
                                                                GroupType = "organisation"
                                                             });

            AddMap<Team>(teams => from team in teams
                                                select new
                                                {
                                                    team.Id,
                                                    team.Name,
                                                    GroupType = "team"
                                                });

            AddMap<Project>(projects => from project in projects
                                  select new
                                  {
                                      project.Id,
                                      project.Name,
                                      GroupType = "project"
                                  });

            AddMap<UserProject>(userProjects => from userProject in userProjects
                                        select new
                                        {
                                            userProject.Id,
                                            userProject.Name,
                                            GroupType = "userproject"
                                        });

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
                    result.Name,
                    result.GroupType,
                    AppRoot = appRoot,
                    Organisation = organisation,
                    Team = team,
                    Project = project,
                    UserProject = userProject
                };

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.GroupType, FieldStorage.Yes);
        }
    }
}