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
    public class All_Species : AbstractMultiMapIndexCreationTask<All_Species.Result>
    {
        public class Result
        {
            public string Taxonomy { get; set; }
            public string Name { get; set; }
            public string RankPosition { get; set; }
            public string RankName { get; set; }
            public string RankType { get; set; }
            public string ParentRankName { get; set; }
            public IDictionary<string, string>[] Ranks { get; set; }
            public string Category { get; set; }
            public string[] CommonGroupNames { get; set; }
            public string[] CommonNames { get; set; }
            public string[] Synonyms { get; set; }
            public int? SpeciesCount { get; set; }
            public object[] AllNames { get; set; }
            public object[] AllScientificNames { get; set; }
            public object[] AllCommonNames { get; set; }
            public string[] AllCategories { get; set; }
            public int? SightingCount { get; set; }
        }

        public All_Species()
        {
            // Rank 1
            AddMap<Species>(species => from s in species
                                       let hasRank1 = s.Taxonomy.ElementAtOrDefault(0) != null
                                       let rank1Name = hasRank1 ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank1Type = hasRank1 ? s.Taxonomy.ElementAt(0).Type : (string)null
                                       select new
                                       {
                                           Taxonomy = hasRank1 ? rank1Name : "[no-rank-found]",
                                           Name = rank1Name,
                                           RankPosition = "1",
                                           RankName = rank1Name,
                                           RankType = rank1Type,
                                           ParentRankName = (string)null,
                                           Ranks = new object []
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0)
                                               },
                                           s.Category,
                                           CommonGroupNames = new string[] {},
                                           CommonNames = new string[] {},
                                           Synonyms = new string[] {},
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   rank1Name
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank1Name
                                               },
                                           AllCommonNames = new object[]
                                               {
                                               },
                                           AllCategories = new [] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 1 observation sightings count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank1 = identification.TaxonomicRanks.ElementAtOrDefault(0) != null
                                                let rank1Name = hasRank1 ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank1 ? rank1Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 2
            AddMap<Species>(species => from s in species
                                       let hasRank2 = s.Taxonomy.ElementAtOrDefault(1) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = hasRank2 ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank2Type = hasRank2 ? s.Taxonomy.ElementAt(1).Type : (string)null
                                       let isFamilyRank = hasRank2 && s.Taxonomy.ElementAt(1).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank2 ? rank1Name + ": " + rank2Name : "[no-rank-found]",
                                           Name = rank2Name,
                                           RankPosition = "2",
                                           RankName = rank2Name,
                                           RankType = rank2Type,
                                           ParentRankName = rank1Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1)
                                               },
                                           s.Category,
                                           CommonGroupNames = new string[] { },
                                           CommonNames = new string[] { },
                                           Synonyms = new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   rank2Name
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank2Name,
                                               },
                                           AllCommonNames = new object[]
                                               {
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 2 observation sightings count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank2 = identification.TaxonomicRanks.ElementAtOrDefault(1) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = hasRank2 ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank2 ? rank1Name + ": " + rank2Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 3
            AddMap<Species>(species => from s in species
                                       let hasRank3 = s.Taxonomy.ElementAtOrDefault(2) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = hasRank3 ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank3Type = hasRank3 ? s.Taxonomy.ElementAt(2).Type : (string)null
                                       let isSpeciesRank = hasRank3 && s.Taxonomy.ElementAt(1).Type == "genus" && s.Taxonomy.ElementAt(2).Type == "species"
                                       let rank3GenusSpeciesName = isSpeciesRank ? rank2Name + " " + rank3Name : (string)null
                                       let isFamilyRank = hasRank3 && s.Taxonomy.ElementAt(2).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank3 ? rank1Name + ": " + rank2Name + ": " + rank3Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank3GenusSpeciesName : rank3Name,
                                           RankPosition = "3",
                                           RankName = rank3Name,
                                           RankType = rank3Type,
                                           ParentRankName = rank2Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank3Name,
                                                   rank3GenusSpeciesName,
                                                   isSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank3Name,
                                                   rank3GenusSpeciesName,
                                                   isSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 3 observation sightings count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank3 = identification.TaxonomicRanks.ElementAtOrDefault(2) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = hasRank3 ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank3 ? rank1Name + ": " + rank2Name + ": " + rank3Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 4
            AddMap<Species>(species => from s in species
                                       let hasRank4 = s.Taxonomy.ElementAtOrDefault(3) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank4Name = hasRank4 ? s.Taxonomy.ElementAt(3).Name : (string)null
                                       let rank4Type = hasRank4 ? s.Taxonomy.ElementAt(3).Type : (string)null
                                       let isSpeciesRank = hasRank4 && s.Taxonomy.ElementAt(2).Type == "genus" && s.Taxonomy.ElementAt(3).Type == "species"
                                       let isSubSpeciesRank = hasRank4 && s.Taxonomy.ElementAt(1).Type == "genus" && s.Taxonomy.ElementAt(2).Type == "species" && s.Taxonomy.ElementAt(3).Type == "subspecies"
                                       let rank4GenusSpeciesName = isSpeciesRank ? rank3Name + " " + rank4Name : (string)null
                                       let rank4GenusSubSpeciesName = isSubSpeciesRank ? rank2Name + " " + rank3Name + " " + rank4Name : (string)null
                                       let isFamilyRank = hasRank4 && s.Taxonomy.ElementAt(3).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank4 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank4GenusSpeciesName : isSubSpeciesRank ? rank4GenusSubSpeciesName : rank4Name,
                                           RankPosition = "4",
                                           RankName = rank4Name,
                                           RankType = rank4Type,
                                           ParentRankName = rank3Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2),
                                                   s.Taxonomy.ElementAtOrDefault(3)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank || isSubSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank4Name,
                                                   rank4GenusSpeciesName,
                                                   rank4GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank4Name,
                                                   rank4GenusSpeciesName,
                                                   rank4GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 4 observation sightings count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank4 = identification.TaxonomicRanks.ElementAtOrDefault(3) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = identification.TaxonomicRanks.ElementAtOrDefault(2) != null ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                let rank4Name = hasRank4 ? identification.TaxonomicRanks.ElementAt(3).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank4 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 5
            AddMap<Species>(species => from s in species
                                       let hasRank5 = s.Taxonomy.ElementAtOrDefault(4) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
                                       let rank5Name = hasRank5 ? s.Taxonomy.ElementAt(4).Name : (string)null
                                       let rank5Type = hasRank5 ? s.Taxonomy.ElementAt(4).Type : (string)null
                                       let isSpeciesRank = hasRank5 && s.Taxonomy.ElementAt(3).Type == "genus" && s.Taxonomy.ElementAt(4).Type == "species"
                                       let isSubSpeciesRank = hasRank5 && s.Taxonomy.ElementAt(2).Type == "genus" && s.Taxonomy.ElementAt(3).Type == "species" && s.Taxonomy.ElementAt(4).Type == "subspecies"
                                       let rank5GenusSpeciesName = isSpeciesRank ? rank4Name + " " + rank5Name : (string)null
                                       let rank5GenusSubSpeciesName = isSubSpeciesRank ? rank3Name + " " + rank4Name + " " + rank5Name : (string)null
                                       let isFamilyRank = hasRank5 && s.Taxonomy.ElementAt(4).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank5 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank5GenusSpeciesName : isSubSpeciesRank ? rank5GenusSubSpeciesName : rank5Name,
                                           RankPosition = "5",
                                           RankName = rank5Name,
                                           RankType = rank5Type,
                                           ParentRankName = rank4Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2),
                                                   s.Taxonomy.ElementAtOrDefault(3),
                                                   s.Taxonomy.ElementAtOrDefault(4)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank || isSubSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank5Name,
                                                   rank5GenusSpeciesName,
                                                   rank5GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank5Name,
                                                   rank5GenusSpeciesName,
                                                   rank5GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 5 observation sightings count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank5 = identification.TaxonomicRanks.ElementAtOrDefault(4) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = identification.TaxonomicRanks.ElementAtOrDefault(2) != null ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                let rank4Name = identification.TaxonomicRanks.ElementAtOrDefault(3) != null ? identification.TaxonomicRanks.ElementAt(3).Name : (string)null
                                                let rank5Name = hasRank5 ? identification.TaxonomicRanks.ElementAt(4).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank5 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 6
            AddMap<Species>(species => from s in species
                                       let hasRank6 = s.Taxonomy.ElementAtOrDefault(5) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
                                       let rank6Name = hasRank6 ? s.Taxonomy.ElementAt(5).Name : (string)null
                                       let rank6Type = hasRank6 ? s.Taxonomy.ElementAt(5).Type : (string)null
                                       let isSpeciesRank = hasRank6 && s.Taxonomy.ElementAt(4).Type == "genus" && s.Taxonomy.ElementAt(5).Type == "species"
                                       let isSubSpeciesRank = hasRank6 && s.Taxonomy.ElementAt(3).Type == "genus" && s.Taxonomy.ElementAt(4).Type == "species" && s.Taxonomy.ElementAt(5).Type == "subspecies"
                                       let rank6GenusSpeciesName = isSpeciesRank ? rank5Name + " " + rank6Name : (string)null
                                       let rank6GenusSubSpeciesName = isSubSpeciesRank ? rank4Name + " " + rank5Name + " " + rank6Name : (string)null
                                       let isFamilyRank = hasRank6 && s.Taxonomy.ElementAt(5).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank6 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank6GenusSpeciesName : isSubSpeciesRank ? rank6GenusSubSpeciesName : rank6Name,
                                           RankPosition = "6",
                                           RankName = rank6Name,
                                           RankType = rank6Type,
                                           ParentRankName = rank5Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2),
                                                   s.Taxonomy.ElementAtOrDefault(3),
                                                   s.Taxonomy.ElementAtOrDefault(4),
                                                   s.Taxonomy.ElementAtOrDefault(5)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank || isSubSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank6Name,
                                                   rank6GenusSpeciesName,
                                                   rank6GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank6Name,
                                                   rank6GenusSpeciesName,
                                                   rank6GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 6 observation sighting count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank6 = identification.TaxonomicRanks.ElementAtOrDefault(5) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = identification.TaxonomicRanks.ElementAtOrDefault(2) != null ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                let rank4Name = identification.TaxonomicRanks.ElementAtOrDefault(3) != null ? identification.TaxonomicRanks.ElementAt(3).Name : (string)null
                                                let rank5Name = identification.TaxonomicRanks.ElementAtOrDefault(4) != null ? identification.TaxonomicRanks.ElementAt(4).Name : (string)null
                                                let rank6Name = hasRank6 ? identification.TaxonomicRanks.ElementAt(5).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank6 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new string[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 7
            AddMap<Species>(species => from s in species
                                       let hasRank7 = s.Taxonomy.ElementAtOrDefault(6) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
                                       let rank6Name = s.Taxonomy.ElementAtOrDefault(5) != null ? s.Taxonomy.ElementAt(5).Name : (string)null
                                       let rank7Name = hasRank7 ? s.Taxonomy.ElementAt(6).Name : (string)null
                                       let rank7Type = hasRank7 ? s.Taxonomy.ElementAt(6).Type : (string)null
                                       let isSpeciesRank = hasRank7 && s.Taxonomy.ElementAt(5).Type == "genus" && s.Taxonomy.ElementAt(6).Type == "species"
                                       let isSubSpeciesRank = hasRank7 && s.Taxonomy.ElementAt(4).Type == "genus" && s.Taxonomy.ElementAt(5).Type == "species" && s.Taxonomy.ElementAt(6).Type == "subspecies"
                                       let rank7GenusSpeciesName = isSpeciesRank ? rank6Name + " " + rank7Name : (string)null
                                       let rank7GenusSubSpeciesName = isSubSpeciesRank ? rank5Name + " " + rank6Name + " " + rank7Name : (string)null
                                       let isFamilyRank = hasRank7 && s.Taxonomy.ElementAt(6).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank7 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank7GenusSpeciesName : isSubSpeciesRank ? rank7GenusSubSpeciesName : rank7Name,
                                           RankPosition = "7",
                                           RankName = rank7Name,
                                           RankType = rank7Type,
                                           ParentRankName = rank6Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2),
                                                   s.Taxonomy.ElementAtOrDefault(3),
                                                   s.Taxonomy.ElementAtOrDefault(4),
                                                   s.Taxonomy.ElementAtOrDefault(5),
                                                   s.Taxonomy.ElementAtOrDefault(6)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank || isSubSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank7Name,
                                                   rank7GenusSpeciesName,
                                                   rank7GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank7Name,
                                                   rank7GenusSpeciesName,
                                                   rank7GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 7 observation sighting count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank7 = identification.TaxonomicRanks.ElementAtOrDefault(6) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = identification.TaxonomicRanks.ElementAtOrDefault(2) != null ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                let rank4Name = identification.TaxonomicRanks.ElementAtOrDefault(3) != null ? identification.TaxonomicRanks.ElementAt(3).Name : (string)null
                                                let rank5Name = identification.TaxonomicRanks.ElementAtOrDefault(4) != null ? identification.TaxonomicRanks.ElementAt(4).Name : (string)null
                                                let rank6Name = identification.TaxonomicRanks.ElementAtOrDefault(5) != null ? identification.TaxonomicRanks.ElementAt(5).Name : (string)null
                                                let rank7Name = hasRank7 ? identification.TaxonomicRanks.ElementAt(6).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank7 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] { },
                                                    identification.Category,
                                                    CommonGroupNames = new string[] { },
                                                    CommonNames = new string[] { },
                                                    Synonyms = new object[] { },
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] { },
                                                    AllScientificNames = new object[] { },
                                                    AllCommonNames = new object[] { },
                                                    AllCategories = new object[] { },
                                                    SightingCount = 1
                                                });

            // Rank 8
            AddMap<Species>(species => from s in species
                                       let hasRank8 = s.Taxonomy.ElementAtOrDefault(7) != null
                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
                                       let rank6Name = s.Taxonomy.ElementAtOrDefault(5) != null ? s.Taxonomy.ElementAt(5).Name : (string)null
                                       let rank7Name = s.Taxonomy.ElementAtOrDefault(6) != null ? s.Taxonomy.ElementAt(6).Name : (string)null
                                       let rank8Name = hasRank8 ? s.Taxonomy.ElementAt(7).Name : (string)null
                                       let rank8Type = hasRank8 ? s.Taxonomy.ElementAt(7).Type : (string)null
                                       let isSpeciesRank = hasRank8 && s.Taxonomy.ElementAt(6).Type == "genus" && s.Taxonomy.ElementAt(7).Type == "species"
                                       let isSubSpeciesRank = hasRank8 && s.Taxonomy.ElementAt(5).Type == "genus" && s.Taxonomy.ElementAt(6).Type == "species" && s.Taxonomy.ElementAt(7).Type == "subspecies"
                                       let rank8GenusSpeciesName = isSpeciesRank ? rank7Name + " " + rank8Name : (string)null
                                       let rank8GenusSubSpeciesName = isSubSpeciesRank ? rank6Name + " " + rank7Name + " " + rank8Name : (string)null
                                       let isFamilyRank = hasRank8 && s.Taxonomy.ElementAt(7).Type == "family"
                                       select new
                                       {
                                           Taxonomy = hasRank8 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name + ": " + rank8Name : "[no-rank-found]",
                                           Name = isSpeciesRank ? rank8GenusSpeciesName : isSubSpeciesRank ? rank8GenusSubSpeciesName : rank8Name,
                                           RankPosition = "8",
                                           RankName = rank8Name,
                                           RankType = rank8Type,
                                           ParentRankName = rank7Name,
                                           Ranks = new object[]
                                               {
                                                   s.Taxonomy.ElementAtOrDefault(0),
                                                   s.Taxonomy.ElementAtOrDefault(1),
                                                   s.Taxonomy.ElementAtOrDefault(2),
                                                   s.Taxonomy.ElementAtOrDefault(3),
                                                   s.Taxonomy.ElementAtOrDefault(4),
                                                   s.Taxonomy.ElementAtOrDefault(5),
                                                   s.Taxonomy.ElementAtOrDefault(6),
                                                   s.Taxonomy.ElementAtOrDefault(7)
                                               },
                                           s.Category,
                                           CommonGroupNames = isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] { },
                                           CommonNames = isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] { },
                                           Synonyms = isSpeciesRank || isSubSpeciesRank ? new[] { s.Synonym } : new string[] { },
                                           SpeciesCount = 1,
                                           AllNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                                   rank8Name,
                                                   rank8GenusSpeciesName,
                                                   rank8GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllScientificNames = new object[]
                                               {
                                                   rank8Name,
                                                   rank8GenusSpeciesName,
                                                   rank8GenusSubSpeciesName,
                                                   isSpeciesRank || isSubSpeciesRank ? s.Synonym : (string)null
                                               },
                                           AllCommonNames = new object[]
                                               {
                                                   isFamilyRank || isSpeciesRank || isSubSpeciesRank ? s.CommonGroupNames : new string[] {},
                                                   isSpeciesRank || isSubSpeciesRank ? s.CommonNames : new string[] {},
                                               },
                                           AllCategories = new[] { s.Category },
                                           SightingCount = 0
                                       });

            // Rank 8 observation sighting count
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                let hasRank8 = identification.TaxonomicRanks.ElementAtOrDefault(7) != null
                                                let rank1Name = identification.TaxonomicRanks.ElementAtOrDefault(0) != null ? identification.TaxonomicRanks.ElementAt(0).Name : (string)null
                                                let rank2Name = identification.TaxonomicRanks.ElementAtOrDefault(1) != null ? identification.TaxonomicRanks.ElementAt(1).Name : (string)null
                                                let rank3Name = identification.TaxonomicRanks.ElementAtOrDefault(2) != null ? identification.TaxonomicRanks.ElementAt(2).Name : (string)null
                                                let rank4Name = identification.TaxonomicRanks.ElementAtOrDefault(3) != null ? identification.TaxonomicRanks.ElementAt(3).Name : (string)null
                                                let rank5Name = identification.TaxonomicRanks.ElementAtOrDefault(4) != null ? identification.TaxonomicRanks.ElementAt(4).Name : (string)null
                                                let rank6Name = identification.TaxonomicRanks.ElementAtOrDefault(5) != null ? identification.TaxonomicRanks.ElementAt(5).Name : (string)null
                                                let rank7Name = identification.TaxonomicRanks.ElementAtOrDefault(6) != null ? identification.TaxonomicRanks.ElementAt(6).Name : (string)null
                                                let rank8Name = hasRank8 ? identification.TaxonomicRanks.ElementAt(7).Name : (string)null
                                                select new
                                                {
                                                    Taxonomy = hasRank8 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name + ": " + rank8Name : "[no-rank-found]",
                                                    Name = (string)null,
                                                    RankPosition = (string)null,
                                                    RankName = (string)null,
                                                    RankType = (string)null,
                                                    ParentRankName = (string)null,
                                                    Ranks = new object[] {},
                                                    identification.Category,
                                                    CommonGroupNames = new string[] {},
                                                    CommonNames = new string[] {},
                                                    Synonyms = new object[] {},
                                                    SpeciesCount = 0,
                                                    AllNames = new object[] {},
                                                    AllScientificNames = new object[] {},
                                                    AllCommonNames = new object[] {},
                                                    AllCategories = new object[] {},
                                                    SightingCount = 1
                                                });

            Reduce = results => from result in results
                                where result.Taxonomy != "[no-rank-found]"
                                group result by result.Taxonomy
                                    into g
                                    select new
                                    {
                                        Taxonomy = g.Key,
                                        Name = g.Select(x => x.Name).Where(x => x != null).FirstOrDefault(),
                                        RankPosition = g.Select(x => x.RankPosition).Where(x => x != null).FirstOrDefault(),
                                        RankName = g.Select(x => x.RankName).Where(x => x != null).FirstOrDefault(),
                                        RankType = g.Select(x => x.RankType).Where(x => x != null).FirstOrDefault(),
                                        ParentRankName = g.Select(x => x.ParentRankName).Where(x => x != null).FirstOrDefault(),
                                        Ranks = g.SelectMany(x => x.Ranks).Distinct(),
                                        Category = g.SelectMany(x => x.AllCategories).Distinct().Count() > 1 ? (string)null : g.SelectMany(x => x.AllCategories).FirstOrDefault(),
                                        CommonGroupNames = g.SelectMany(x => x.CommonGroupNames).Distinct(),
                                        CommonNames = g.SelectMany(x => x.CommonNames).Distinct(),
                                        Synonyms = g.SelectMany(x => x.Synonyms).Distinct(),
                                        SpeciesCount = g.Sum(x => x.SpeciesCount),
                                        AllNames = g.SelectMany(x => x.AllNames),
                                        AllScientificNames = g.SelectMany(x => x.AllScientificNames),
                                        AllCommonNames = g.SelectMany(x => x.AllCommonNames),
                                        AllCategories = g.SelectMany(x => x.AllCategories),
                                        SightingCount = g.Sum(x => x.SightingCount)
                                    };

            TransformResults = (database, results) =>
                                from result in results
                                select new
                                {
                                    result.Taxonomy,
                                    result.Name,
                                    result.RankPosition,
                                    result.RankName,
                                    result.RankType,
                                    result.ParentRankName,
                                    result.Ranks,
                                    result.Category,
                                    CommonGroupNames = result.CommonGroupNames.Distinct(),
                                    CommonNames = result.CommonNames.Distinct(),
                                    Synonyms = result.Synonyms.Distinct(),
                                    result.SpeciesCount,
                                    result.AllNames,
                                    result.AllScientificNames,
                                    result.AllCommonNames,
                                    result.AllCategories,
                                    result.SightingCount
                                };

            Store(x => x.Taxonomy, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.RankPosition, FieldStorage.Yes);
            Store(x => x.RankName, FieldStorage.Yes);
            Store(x => x.RankType, FieldStorage.Yes);
            Store(x => x.ParentRankName, FieldStorage.Yes);
            Store(x => x.Ranks, FieldStorage.Yes);
            Store(x => x.Category, FieldStorage.Yes);
            Store(x => x.CommonGroupNames, FieldStorage.Yes);
            Store(x => x.CommonNames, FieldStorage.Yes);
            Store(x => x.Synonyms, FieldStorage.Yes);
            Store(x => x.SpeciesCount, FieldStorage.Yes);
            Store(x => x.SightingCount, FieldStorage.Yes);
            Store(x => x.AllNames, FieldStorage.No);
            Store(x => x.AllScientificNames, FieldStorage.No);
            Store(x => x.AllCommonNames, FieldStorage.No);
            Store(x => x.AllCategories, FieldStorage.No);
        } 
    }
}