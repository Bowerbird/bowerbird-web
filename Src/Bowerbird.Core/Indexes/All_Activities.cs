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
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Connection;

namespace Bowerbird.Core.Indexes
{
    public class All_Activities
    {
        public class Result
        {
            public string Type { get; set; }
            public string UserId { get; set; }
            public string[] GroupIds { get; set; }
            public DateTime CreatedDateTime { get; set; }
        }

        public static void Create(IDocumentStore documentStore)
        {
            var indexNames = documentStore.DatabaseCommands.GetIndexNames(1, 100);

            if (!documentStore.DatabaseCommands.GetIndexNames(0, 100).Any(x => x == "All/Activities"))
            {
                documentStore.DatabaseCommands.PutIndex("All/Activities",
                new IndexDefinition
                {
                    Map = @"
                        from activity in docs.Activities
                        select new { Type = activity.Type, CreatedDateTime = activity.CreatedDateTime, UserId = activity.User.Id, GroupIds = activity.Groups.Select(x => x.Id) };
                        ",

                    Stores = new Dictionary<string, FieldStorage> {
                        { "Type", FieldStorage.Yes },
                        { "UserId", FieldStorage.Yes },
                        { "GroupIds", FieldStorage.Yes },
                        { "CreatedDateTime", FieldStorage.Yes }
                    }
                });
            }
        }
    }
}