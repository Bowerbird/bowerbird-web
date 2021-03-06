﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
            public IDictionary<string, string> GroupRoles { get; set; } // Key is role, and value is project
            public string[] ConnectionIds { get; set; }
            public int SightingCount { get; set; }
            public IEnumerable<string> FollowingUserIds { get; set; }
            public DateTime[] LatestHeartbeat { get; set; }
            public DateTime[] LatestActivity { get; set; }
            public string[] LatestObservationIds { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Description { get; set; }
            public object[] AllFields { get; set; }
            public string SortName { get; set; }
            public int FollowerCount { get; set; }

            public User User { get; set; }
            public IEnumerable<UserProject> UserProjects { get; set; }
            public IEnumerable<Favourites> Favourites { get; set; }
            public IEnumerable<Project> Projects { get; set; }
            public IEnumerable<Organisation> Organisations { get; set; }
            public IEnumerable<AppRoot> AppRoots { get; set; }
            public IEnumerable<Observation> LatestObservations { get; set; } 

            public IEnumerable<Group> Groups
            {
                get
                {
                    List<Group> groups = new List<Group>();
                    if (UserProjects != null) groups.AddRange(UserProjects);
                    if (Favourites != null) groups.AddRange(Favourites);
                    if (Projects != null) groups.AddRange(Projects);
                    if (Organisations != null) groups.AddRange(Organisations);
                    if (AppRoots != null) groups.AddRange(AppRoots);
                    return groups;
                }
            }
        }

        public All_Users()
        {
            // NOTE: This index uses Dynamic fields: http://ravendb.net/docs/client-api/advanced/dynamic-fields
            // Discussion about how to create dynamic fields when you are doing a map AND reduce: https://groups.google.com/forum/?fromgroups=#!searchin/ravendb/dynamic$20fields/ravendb/EX46ycd2LO4/3isaSTB8EGYJ
            // Note the weird "_ = "ignored",". It makes creating fields possible in the REDUCE bit.
            // To query the dynamic fields, you must use a LuceneQuery<>() as the dynamic fields are not Linq queryable properties.
            // Have fun.

            AddMap<User>(users => from user in users
                                      select new
                                      {
                                          UserId = user.Id,
                                          GroupIds = user.Memberships.Select(x => x.Group.Id),
                                          GroupRoles = user.Memberships.SelectMany(x => x.Roles.Select(y => new { Key = y.Id.Replace("roles/", ""), Value = x.Group.Id})),
                                          _ = "ignored",
                                          ConnectionIds = user.Sessions.Select(x => x.ConnectionId),
                                          SightingCount = 0,
                                          FollowingUserIds = new string[] {},
                                          LatestHeartbeat = user.Sessions.Select(x => x.LatestHeartbeat),
                                          LatestActivity = user.Sessions.Select(x => x.LatestActivity),
                                          LatestObservationIds = new string[] { },
                                          user.Name,
                                          user.Email,
                                          user.Description,
                                          AllFields = new []
                                              {
                                                  user.Name,
                                                  user.Description
                                              },
                                          SortName = user.Name
                                      });

            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                {
                                                    UserId = observation.User.Id,
                                                    GroupIds = new string [] {},
                                                    GroupRoles = new object[] { },
                                                    _ = "ignored",
                                                    ConnectionIds = new string[] { },
                                                    SightingCount = 1,
                                                    FollowingUserIds = new string[] { },
                                                    LatestHeartbeat = new object [] {},
                                                    LatestActivity = new object[] { },
                                                    LatestObservationIds = new[] { observation.Id },
                                                    Name = (string)null,
                                                    Email = (string)null,
                                                    Description = (string)null,
                                                    AllFields = new object[] { },
                                                    SortName = (string)null
                                                });

            AddMap<Record>(records => from record in records
                                      select new
                                      {
                                          UserId = record.User.Id,
                                          GroupIds = new string[] { },
                                          GroupRoles = new object[] { },
                                          _ = "ignored",
                                          ConnectionIds = new string[] { },
                                          SightingCount = 1,
                                          FollowingUserIds = new string[] { },
                                          LatestHeartbeat = new object[] { },
                                          LatestActivity = new object[] { },
                                          LatestObservationIds = new string[] { },
                                          Name = (string)null,
                                          Email = (string)null,
                                          Description = (string)null,
                                          AllFields = new object[] { },
                                          SortName = (string)null
                                      });

            // Get followers
            AddMap<User>(users => from user in users
                                  select new
                                      {
                                          UserId = user.Id,
                                          GroupIds = new string[] { },
                                          GroupRoles = new object[] { },
                                          _ = "ignored",
                                          ConnectionIds = new string[] { },
                                          SightingCount = 0,
                                          FollowingUserIds = user.Memberships.Where(x => x.Group.GroupType == "userproject" && x.Group.CreatedBy != user.Id).Select(x => x.Group.CreatedBy),
                                          LatestHeartbeat = new object[] { },
                                          LatestActivity = new object[] { },
                                          LatestObservationIds = new string[] { },
                                          Name = (string)null,
                                          Email = (string)null,
                                          Description = (string)null,
                                          AllFields = new object[] { },
                                          SortName = (string)null
                                      });

            Reduce = results => from result in results
                                group result by result.UserId
                                    into g
                                    select new
                                    {
                                          UserId = g.Key,
                                          GroupIds = g.SelectMany(x => x.GroupIds).Distinct(),
                                          GroupRoles = g.SelectMany(x => x.GroupRoles),
                                          _ = g.SelectMany(x => x.GroupRoles).Select(y => CreateField(y.Key, y.Value)),
                                          ConnectionIds = g.SelectMany(x => x.ConnectionIds).Distinct(),
                                          SightingCount = g.Sum(x => x.SightingCount),
                                          FollowingUserIds = g.SelectMany(x => x.FollowingUserIds),
                                          LatestHeartbeat = g.SelectMany(x => x.LatestHeartbeat),
                                          LatestActivity = g.SelectMany(x => x.LatestActivity),
                                          LatestObservationIds = g.SelectMany(x => x.LatestObservationIds).OrderByDescending(x => x).Take(10), // Just ordering by string, lexicographically
                                          Name = g.Select(x => x.Name).Where(x => x != null).FirstOrDefault(),
                                          Email = g.Select(x => x.Email).Where(x => x != null).FirstOrDefault(),
                                          Description = g.Select(x => x.Description).Where(x => x != null).FirstOrDefault(),
                                          AllFields = g.SelectMany(x => x.AllFields),
                                          SortName = g.Select(x => x.Name).Where(x => x != null).FirstOrDefault()
                                    };

            TransformResults = (database, results) =>
                                from result in results
                                select new
                                {
                                    result.UserId,
                                    GroupIds = result.GroupIds ?? new string[] { },
                                    //GroupRoles = result.GroupRoles.Select(x => new { x.Key, x.Value }),
                                    ConnectionIds = result.ConnectionIds ?? new string[] { },
                                    result.SightingCount,
                                    result.FollowingUserIds,
                                    LatestHeartbeat = result.LatestHeartbeat ?? new DateTime[] { },
                                    LatestActivity = result.LatestActivity ?? new DateTime[] { },
                                    FollowerCount = result.FollowingUserIds.Count(),
                                    User = database.Load<User>(result.UserId),
                                    UserProjects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "userproject"),
                                    Favourites = database.Load<Favourites>(result.GroupIds).Where(x => x.GroupType == "favourites"),
                                    Projects = database.Load<Project>(result.GroupIds).Where(x => x.GroupType == "project"),
                                    Organisations = database.Load<Organisation>(result.GroupIds).Where(x => x.GroupType == "organisation"),
                                    AppRoots = database.Load<AppRoot>(result.GroupIds).Where(x => x.GroupType == "approot"),
                                    LatestObservations = database.Load<Observation>(result.LatestObservationIds)
                                };

            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
            Store(x => x.GroupRoles, FieldStorage.Yes);
            Store(x => x.ConnectionIds, FieldStorage.Yes);
            Store(x => x.SightingCount, FieldStorage.Yes);
            Store(x => x.FollowingUserIds, FieldStorage.Yes);
            Store(x => x.LatestHeartbeat, FieldStorage.Yes);
            Store(x => x.LatestActivity, FieldStorage.Yes);
            Store(x => x.LatestObservationIds, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.Email, FieldStorage.Yes);
            Store(x => x.Description, FieldStorage.Yes);
            Store(x => x.AllFields, FieldStorage.Yes);
            Store(x => x.SortName, FieldStorage.Yes);
            
            Index(x => x.Name, FieldIndexing.Analyzed);
            Index(x => x.Description, FieldIndexing.Analyzed);
            Index(x => x.AllFields, FieldIndexing.Analyzed);
        }
    }
}