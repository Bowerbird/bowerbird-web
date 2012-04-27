/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Bowerbird.Core.Indexes
{
    public class All_Projects : AbstractMultiMapIndexCreationTask<All_Projects.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public IEnumerable<string> GroupId { get; set; }
            public int MemberCount { get; set; }
            public int ObservationCount { get; set; }
            public int ProjectCount { get; set; }
        }

        public All_Projects()
        {
            AddMap<Project>(projects => from project in projects
                                        select new
                                        {
                                            Id = project.Id,
                                            Type = "project",
                                            GroupId = new string[]{project.Id},
                                            MemberCount = 0,
                                            ObservationCount = 0,
                                            ProjectCount = 1
                                        });


            AddMap<Member>(members => from member in members
                                      select new
                                      {
                                          Id = member.Id,
                                          Type = "member",
                                          GroupId = new string[]{member.Group.Id},
                                          MemberCount = 1,
                                          ObservationCount = 0,
                                          ProjectCount = 0
                                      });

            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                {
                                                    Id = observation.Id,
                                                    Type = "observation",
                                                    GroupId = observation.Groups.Select(x => x.GroupId).Where(x => !x.Contains("userprojects")).ToArray(),
                                                    MemberCount = 0,
                                                    ObservationCount = 1,
                                                    ProjectCount = 0
                                                });

            Reduce = results => from result in results
                                group result by result.Id
                                into g
                                select new
                                           {
                                               Id = g.Key,
                                               g.Where(x => x.Type != null).First().Type,
                                               MemberCount = g.Where(x => x.Type == "member" && x.GroupId.Contains(g.Key)).Sum(x => x.MemberCount),
                                               ObservationCount = g.Where(x => x.Type == "observation" && x.GroupId.Contains(g.Key)).Sum(x => x.ObservationCount),
                                               ProjectCount = g.Where(x => x.Type == "project").Sum(x => x.ProjectCount),
                                               GroupId = new string []{g.Key}
                                           };

            //Reduce = results => from result in results
            //                    group result by result.Id
            //                        into g
            //                        select new
            //                        {
            //                            Id = g.Key,
            //                            Type = g.Where(x => x.Type == "project").First().Type,
            //                            MemberCount = g.Where(x => x.Type == "member" && x.GroupId.Contains(g.Key)).Sum(x => x.MemberCount),
            //                            ObservationCount = g.Where(x => x.Type == "observation" && x.GroupId.Contains(g.Key)).Sum(x => x.ObservationCount),
            //                            ProjectCount = g.Where(x => x.Type == "project").Sum(x => x.ProjectCount),
            //                            GroupId = new string[] { g.Key }
            //                        };

            TransformResults = (database, results) =>
                from result in results
                let observation = database.Load<Observation>(result.Id)
                select new
                {
                    result.Id,
                    result.Type,
                    result.MemberCount,
                    result.ObservationCount,
                    result.ProjectCount,
                    result.GroupId
                };

            Store(x => x.Type, FieldStorage.Yes);
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.MemberCount, FieldStorage.Yes);
            Store(x => x.ObservationCount, FieldStorage.Yes);
            Store(x => x.ProjectCount, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
        }
    }
}