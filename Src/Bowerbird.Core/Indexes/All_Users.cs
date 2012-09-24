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
using System.Collections.Generic;
using System;

namespace Bowerbird.Core.Indexes
{
    public class All_Users : AbstractMultiMapIndexCreationTask<All_Users.Result>
    {
        public class Result
        {
            public string UserId { get; set; }
            public string[] GroupIds { get; set; }
            public string[] ConnectionIds { get; set; }
            public DateTime[] LatestHeartbeat { get; set; }
            public DateTime[] LatestActivity { get; set; }

            public User User { get; set; }
            public IEnumerable<UserProject> UserProjects { get; set; }
            public IEnumerable<Project> Projects { get; set; }
            public IEnumerable<Team> Teams { get; set; }
            public IEnumerable<Organisation> Organisations { get; set; }
            public IEnumerable<AppRoot> AppRoots { get; set; }
            public IEnumerable<Group> Groups
            {
                get
                {
                    List<Group> groups = new List<Group>();
                    if (UserProjects != null && UserProjects.Count() > 0) groups.AddRange(UserProjects);
                    if (Projects != null && Projects.Count() > 0) groups.AddRange(Projects);
                    if (Teams != null && Teams.Count() > 0) groups.AddRange(Teams);
                    if (Organisations != null && Organisations.Count() > 0) groups.AddRange(Organisations);
                    if (AppRoots != null && AppRoots.Count() > 0) groups.AddRange(AppRoots);
                    return groups;
                }
            }
        }

        public All_Users()
        {
            AddMap<User>(users => from user in users
                                      select new
                                      {
                                          UserId = user.Id,
                                          GroupIds = user.Memberships.Select(x => x.Group.Id),
                                          ConnectionIds = user.Sessions.Select(x => x.ConnectionId),
                                          LatestHeartbeat = user.Sessions.Select(x => x.LatestHeartbeat),
                                          LatestInteractivity = user.Sessions.Select(x => x.LatestActivity)
                                      });

            TransformResults = (database, results) =>
                                from result in results
                                select new
                                {
                                    result.UserId,
                                    GroupIds = result.GroupIds ?? new string [] {},
                                    ConnectionIds = result.ConnectionIds ?? new string[] {},
                                    result.LatestHeartbeat,
                                    result.LatestActivity,
                                    User = database.Load<User>(result.UserId),
                                    UserProjects = database.Load<UserProject>(result.GroupIds),
                                    Projects = database.Load<Project>(result.GroupIds),
                                    Teams = database.Load<Team>(result.GroupIds),
                                    Organsations = database.Load<Organisation>(result.GroupIds),
                                    AppRoots = database.Load<AppRoot>(result.GroupIds)
                                };

            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
            Store(x => x.ConnectionIds, FieldStorage.Yes);
            Store(x => x.LatestHeartbeat, FieldStorage.Yes);
            Store(x => x.LatestActivity, FieldStorage.Yes);
        }
    }
}