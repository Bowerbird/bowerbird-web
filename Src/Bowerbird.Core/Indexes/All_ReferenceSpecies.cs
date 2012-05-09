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
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_ReferenceSpecies : AbstractMultiMapIndexCreationTask<All_ReferenceSpecies.Result>
    {
        public class Result
        {
            public string GroupId { get; set; }
            public string SpeciesId { get; set; }
            public string Kingdom { get; set; }
            public string Group { get; set; }
            public IEnumerable<string> CommonNames { get; set; }
            public string Taxonomy { get; set; }
            public string Order { get; set; }
            public string Family { get; set; }
            public string GenusName { get; set; }
            public string SpeciesName { get; set; }
            public DateTime SpeciesCreatedOn { get; set; }
            public IEnumerable<string> SmartTags { get; set; }
            public DateTime ReferenceSpeciesCreatedOn { get; set; }
            public string ReferencedByUserId { get; set; }
            public int ReferenceSpeciesCount { get; set; }
        }

        public All_ReferenceSpecies()
        {
            AddMap<Species>(species => from s in species
                select new
                {
                    GroupId = (string)null,
                    SpeciesId = s.Id,
                    s.Kingdom,
                    s.Group,
                    s.CommonNames,
                    s.Taxonomy,
                    s.Order,
                    s.Family,
                    s.GenusName,
                    s.SpeciesName,
                    SpeciesCreatedOn = s.CreatedDateTime,
                    ReferenceSpeciesCreatedOn = (string)null,
                    SmartTags = (string)null,
                    ReferencedByUserId = (string)null,
                    ReferenceSpeciesCount = 0
                });

            AddMap<ReferenceSpecies>(species => from s in species
                                       select new
                                       {
                                           GroupId = s.GroupId,
                                           SpeciesId = s.SpeciesId,
                                           Kingdom = (string)null,
                                           Group = (string)null,
                                           CommonNames = (string)null,
                                           Taxonomy = (string)null,
                                           Order = (string)null,
                                           Family = (string)null,
                                           GenusName = (string)null,
                                           SpeciesName = (string)null,
                                           SpeciesCreatedOn = (string)null,
                                           ReferenceSpeciesCreatedOn = s.CreatedDateTime,
                                           SmartTags = s.SmartTags,
                                           ReferencedByUserId = s.User.Id,
                                           ReferenceSpeciesCount = 1
                                       });

            Reduce = results => from result in results
                                group result by result.GroupId
                                    into g
                                    select new
                                    {
                                        GroupId = g.Key,
                                        SpeciesId = g.Select(x => x.SpeciesId).FirstOrDefault(),
                                        Kingdom = g.Select(x => x.Kingdom).FirstOrDefault(),
                                        Group = g.Select(x => x.Group).FirstOrDefault(),
                                        CommonNames = g.Select(x => x.CommonNames).FirstOrDefault(),
                                        Taxonomy = g.Select(x => x.Taxonomy).FirstOrDefault(),
                                        Order = g.Select(x => x.Order).FirstOrDefault(),
                                        Family = g.Select(x => x.Family).FirstOrDefault(),
                                        GenusName = g.Select(x => x.GenusName).FirstOrDefault(),
                                        SpeciesName = g.Select(x => x.SpeciesName).FirstOrDefault(),
                                        SpeciesCreatedOn = g.Select(x => x.SpeciesCreatedOn).FirstOrDefault(),
                                        ReferenceSpeciesCreatedOn = g.Select(x => x.ReferenceSpeciesCreatedOn).FirstOrDefault(),
                                        SmartTags = g.Select(x => x.SmartTags).FirstOrDefault(),
                                        ReferencedByUserId = g.Select(x => x.ReferencedByUserId).FirstOrDefault(),
                                        ReferenceSpeciesCount = g.Sum(x => x.ReferenceSpeciesCount),
                                    };

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.GroupId,
                    result.SpeciesId,
                    result.Kingdom,
                    result.Group,
                    result.CommonNames,
                    result.Taxonomy,
                    result.Order,
                    result.Family,
                    result.GenusName,
                    result.SpeciesName,
                    result.SpeciesCreatedOn,
                    result.ReferenceSpeciesCreatedOn,
                    result.SmartTags,
                    result.ReferencedByUserId,
                    result.ReferenceSpeciesCount
                };

            Store(x => x.SpeciesId, FieldStorage.Yes);
            Store(x => x.Kingdom, FieldStorage.Yes);
            Store(x => x.Group, FieldStorage.Yes);
            Store(x => x.CommonNames, FieldStorage.Yes);
            Store(x => x.Taxonomy, FieldStorage.Yes);
            Store(x => x.Order, FieldStorage.Yes);
            Store(x => x.Family, FieldStorage.Yes);
            Store(x => x.GenusName, FieldStorage.Yes);
            Store(x => x.SpeciesName, FieldStorage.Yes);
            Store(x => x.SpeciesCreatedOn, FieldStorage.Yes);
            Store(x => x.ReferenceSpeciesCreatedOn, FieldStorage.Yes);
            Store(x => x.SmartTags, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.ReferencedByUserId, FieldStorage.Yes);
            Store(x => x.ReferenceSpeciesCount, FieldStorage.Yes);
        }
    }
}