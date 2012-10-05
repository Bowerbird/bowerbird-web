///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com

// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au

// Funded by:
// * Atlas of Living Australia

//*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bowerbird.Core.DomainModels;
//using Raven.Abstractions.Indexing;
//using Raven.Client.Indexes;

//namespace Bowerbird.Core.Indexes
//{
//    public class All_CommonNameTaxa : AbstractMultiMapIndexCreationTask<All_CommonNameTaxa.Result>
//    {
//        public class Result
//        {
//            public string Taxonomy { get; set; }
//            public string Name { get; set; }
//            public string RankPosition { get; set; }
//            public string RankName { get; set; }
//            public string RankType { get; set; }
//            public string ParentRankName { get; set; }
//            public IDictionary<string, string> Rank1 { get; set; }
//            public IDictionary<string, string> Rank2 { get; set; }
//            public IDictionary<string, string> Rank3 { get; set; }
//            public IDictionary<string, string> Rank4 { get; set; }
//            public IDictionary<string, string> Rank5 { get; set; }
//            public IDictionary<string, string> Rank6 { get; set; }
//            public IDictionary<string, string> Rank7 { get; set; }
//            public IDictionary<string, string> Rank8 { get; set; }
//            public string Category { get; set; }
//            public string[] CommonGroupNames { get; set; }
//            public string[] CommonNames { get; set; }
//            public string[] SpeciesIds { get; set; }
//            public object[] AllNames { get; set; }
//            public object[] AllScientificNames { get; set; }
//            public object[] AllCommonNames { get; set; }
//            public IEnumerable<Species> Species { get; set; }
//        }

//        public All_CommonNameTaxa()
//        {
//            // Rank 1
//            AddMap<Species>(species => from s in species
//                                       let hasRank1 = s.Taxonomy.ElementAtOrDefault(0) != null
//                                       let rank1Name = hasRank1 ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank1Type = hasRank1 ? s.Taxonomy.ElementAt(0).Type : (string)null
//                                       from commonGroupName in s.CommonGroupNames
//                                       group s by new { commonGroupName, rank1Name, hasRank1, rank1Type } into g
//                                       let category = g.Select(x => x.Category).FirstOrDefault()
//                                       let synonym = g.Select(x => x.Synonym).FirstOrDefault()
//                                       select new
//                                       {
//                                           CommonTaxonomy = category + ": " + g.Key.commonGroupName,
//                                           ScientificTaxonomy = g.Key.hasRank1 ? g.Key.rank1Name : "[no-rank-found]",
//                                           Name = g.Key.rank1Name,
//                                           RankPosition = "1",
//                                           RankName = g.Key.rank1Name,
//                                           RankType = g.Key.rank1Type,
//                                           ParentRankName = (string)null,
//                                           Rank1 = g.Select(x => x.Taxonomy).ElementAtOrDefault(0),
//                                           Rank2 = (object)null,
//                                           Rank3 = (object)null,
//                                           Rank4 = (object)null,
//                                           Rank5 = (object)null,
//                                           Rank6 = (object)null,
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           Category = category,
//                                           //s.CommonGroupNames,
//                                           //s.CommonNames,
//                                           SpeciesIds = new[] { g.Select(x => x.Id).FirstOrDefault() },
//                                           AllNames = new object[]
//                                               {
//                                                   //s.CommonGroupNames,
//                                                   //s.CommonNames,
//                                                   g.Key.rank1Name,
//                                                   synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   g.Key.rank1Name,
//                                                   synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   //s.CommonGroupNames,
//                                                   //s.CommonNames
//                                               }
//                                       });

//            // Rank 2
//            AddMap<Species>(species => from s in species
//                                       let hasRank2 = s.Taxonomy.ElementAtOrDefault(1) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = hasRank2 ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank2Type = hasRank2 ? s.Taxonomy.ElementAt(1).Type : (string)null
//                                       let isSpeciesRank = hasRank2 && s.Taxonomy.ElementAt(0).Type == "genus" && s.Taxonomy.ElementAt(1).Type == "species"
//                                       let rank2GenusSpeciesName = isSpeciesRank ? rank1Name + " " + rank2Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank2 ? rank1Name + ": " + rank2Name : "[no-rank-found]",
//                                           Name = rank2Name,
//                                           RankPosition = "2",
//                                           RankName = rank2Name,
//                                           RankType = rank2Type,
//                                           ParentRankName = rank1Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = (object)null,
//                                           Rank4 = (object)null,
//                                           Rank5 = (object)null,
//                                           Rank6 = (object)null,
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank2GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank2GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 3
//            AddMap<Species>(species => from s in species
//                                       let hasRank3 = s.Taxonomy.ElementAtOrDefault(2) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = hasRank3 ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank3Type = hasRank3 ? s.Taxonomy.ElementAt(2).Type : (string)null
//                                       let isSpeciesRank = hasRank3 && s.Taxonomy.ElementAt(1).Type == "genus" && s.Taxonomy.ElementAt(2).Type == "species"
//                                       let isSubSpeciesRank = hasRank3 && s.Taxonomy.ElementAt(0).Type == "genus" && s.Taxonomy.ElementAt(1).Type == "species" && s.Taxonomy.ElementAt(2).Type == "subspecies"
//                                       let rank3GenusSpeciesName = isSpeciesRank ? rank2Name + " " + rank3Name : (string)null
//                                       let rank3GenusSubSpeciesName = isSubSpeciesRank ? rank1Name + " " + rank2Name + " " + rank3Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank3 ? rank1Name + ": " + rank2Name + ": " + rank3Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank3GenusSpeciesName : isSubSpeciesRank ? rank3GenusSubSpeciesName : rank3Name,
//                                           RankPosition = "3",
//                                           RankName = rank3Name,
//                                           RankType = rank3Type,
//                                           ParentRankName = rank2Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = (object)null,
//                                           Rank5 = (object)null,
//                                           Rank6 = (object)null,
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank3GenusSpeciesName,
//                                                   rank3GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank3GenusSpeciesName,
//                                                   rank3GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 4
//            AddMap<Species>(species => from s in species
//                                       let hasRank4 = s.Taxonomy.ElementAtOrDefault(3) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank4Name = hasRank4 ? s.Taxonomy.ElementAt(3).Name : (string)null
//                                       let rank4Type = hasRank4 ? s.Taxonomy.ElementAt(3).Type : (string)null
//                                       let isSpeciesRank = hasRank4 && s.Taxonomy.ElementAt(2).Type == "genus" && s.Taxonomy.ElementAt(3).Type == "species"
//                                       let isSubSpeciesRank = hasRank4 && s.Taxonomy.ElementAt(1).Type == "genus" && s.Taxonomy.ElementAt(2).Type == "species" && s.Taxonomy.ElementAt(3).Type == "subspecies"
//                                       let rank4GenusSpeciesName = isSpeciesRank ? rank3Name + " " + rank4Name : (string)null
//                                       let rank4GenusSubSpeciesName = isSubSpeciesRank ? rank2Name + " " + rank3Name + " " + rank4Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank4 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank4GenusSpeciesName : isSubSpeciesRank ? rank4GenusSubSpeciesName : rank4Name,
//                                           RankPosition = "4",
//                                           RankName = rank4Name,
//                                           RankType = rank4Type,
//                                           ParentRankName = rank3Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = s.Taxonomy.ElementAtOrDefault(3),
//                                           Rank5 = (object)null,
//                                           Rank6 = (object)null,
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank4GenusSpeciesName,
//                                                   rank4GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank4GenusSpeciesName,
//                                                   rank4GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 5
//            AddMap<Species>(species => from s in species
//                                       let hasRank5 = s.Taxonomy.ElementAtOrDefault(4) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
//                                       let rank5Name = hasRank5 ? s.Taxonomy.ElementAt(4).Name : (string)null
//                                       let rank5Type = hasRank5 ? s.Taxonomy.ElementAt(4).Type : (string)null
//                                       let isSpeciesRank = hasRank5 && s.Taxonomy.ElementAt(3).Type == "genus" && s.Taxonomy.ElementAt(4).Type == "species"
//                                       let isSubSpeciesRank = hasRank5 && s.Taxonomy.ElementAt(2).Type == "genus" && s.Taxonomy.ElementAt(3).Type == "species" && s.Taxonomy.ElementAt(4).Type == "subspecies"
//                                       let rank5GenusSpeciesName = isSpeciesRank ? rank4Name + " " + rank5Name : (string)null
//                                       let rank5GenusSubSpeciesName = isSubSpeciesRank ? rank3Name + " " + rank4Name + " " + rank5Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank5 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank5GenusSpeciesName : isSubSpeciesRank ? rank5GenusSubSpeciesName : rank5Name,
//                                           RankPosition = "5",
//                                           RankName = rank5Name,
//                                           RankType = rank5Type,
//                                           ParentRankName = rank4Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = s.Taxonomy.ElementAtOrDefault(3),
//                                           Rank5 = s.Taxonomy.ElementAtOrDefault(4),
//                                           Rank6 = (object)null,
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank5GenusSpeciesName,
//                                                   rank5GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank5GenusSpeciesName,
//                                                   rank5GenusSubSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 6
//            AddMap<Species>(species => from s in species
//                                       let hasRank6 = s.Taxonomy.ElementAtOrDefault(5) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
//                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
//                                       let rank6Name = hasRank6 ? s.Taxonomy.ElementAt(5).Name : (string)null
//                                       let rank6Type = hasRank6 ? s.Taxonomy.ElementAt(5).Type : (string)null
//                                       let isSpeciesRank = hasRank6 && s.Taxonomy.ElementAt(4).Type == "genus" && s.Taxonomy.ElementAt(5).Type == "species"
//                                       let rank6GenusSpeciesName = isSpeciesRank ? rank5Name + " " + rank6Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank6 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank6GenusSpeciesName : rank6Name,
//                                           RankPosition = "6",
//                                           RankName = rank6Name,
//                                           RankType = rank6Type,
//                                           ParentRankName = rank5Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = s.Taxonomy.ElementAtOrDefault(3),
//                                           Rank5 = s.Taxonomy.ElementAtOrDefault(4),
//                                           Rank6 = s.Taxonomy.ElementAtOrDefault(5),
//                                           Rank7 = (object)null,
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank6GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank6GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 7
//            AddMap<Species>(species => from s in species
//                                       let hasRank7 = s.Taxonomy.ElementAtOrDefault(5) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
//                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
//                                       let rank6Name = s.Taxonomy.ElementAtOrDefault(5) != null ? s.Taxonomy.ElementAt(5).Name : (string)null
//                                       let rank7Name = hasRank7 ? s.Taxonomy.ElementAt(6).Name : (string)null
//                                       let rank7Type = hasRank7 ? s.Taxonomy.ElementAt(6).Type : (string)null
//                                       let isSpeciesRank = hasRank7 && s.Taxonomy.ElementAt(5).Type == "genus" && s.Taxonomy.ElementAt(6).Type == "species"
//                                       let rank7GenusSpeciesName = isSpeciesRank ? rank6Name + " " + rank7Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank7 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank7GenusSpeciesName : rank7Name,
//                                           RankPosition = "7",
//                                           RankName = rank7Name,
//                                           RankType = rank7Type,
//                                           ParentRankName = rank6Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = s.Taxonomy.ElementAtOrDefault(3),
//                                           Rank5 = s.Taxonomy.ElementAtOrDefault(4),
//                                           Rank6 = s.Taxonomy.ElementAtOrDefault(5),
//                                           Rank7 = s.Taxonomy.ElementAtOrDefault(6),
//                                           Rank8 = (object)null,
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank7Name,
//                                                   rank7GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank7Name,
//                                                   rank7GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            // Rank 8
//            AddMap<Species>(species => from s in species
//                                       let hasRank8 = s.Taxonomy.ElementAtOrDefault(5) != null
//                                       let rank1Name = s.Taxonomy.ElementAtOrDefault(0) != null ? s.Taxonomy.ElementAt(0).Name : (string)null
//                                       let rank2Name = s.Taxonomy.ElementAtOrDefault(1) != null ? s.Taxonomy.ElementAt(1).Name : (string)null
//                                       let rank3Name = s.Taxonomy.ElementAtOrDefault(2) != null ? s.Taxonomy.ElementAt(2).Name : (string)null
//                                       let rank4Name = s.Taxonomy.ElementAtOrDefault(3) != null ? s.Taxonomy.ElementAt(3).Name : (string)null
//                                       let rank5Name = s.Taxonomy.ElementAtOrDefault(4) != null ? s.Taxonomy.ElementAt(4).Name : (string)null
//                                       let rank6Name = s.Taxonomy.ElementAtOrDefault(5) != null ? s.Taxonomy.ElementAt(5).Name : (string)null
//                                       let rank7Name = s.Taxonomy.ElementAtOrDefault(6) != null ? s.Taxonomy.ElementAt(6).Name : (string)null
//                                       let rank8Name = hasRank8 ? s.Taxonomy.ElementAt(7).Name : (string)null
//                                       let rank8Type = hasRank8 ? s.Taxonomy.ElementAt(7).Type : (string)null
//                                       let isSpeciesRank = hasRank8 && s.Taxonomy.ElementAt(6).Type == "genus" && s.Taxonomy.ElementAt(7).Type == "species"
//                                       let rank8GenusSpeciesName = isSpeciesRank ? rank7Name + " " + rank8Name : (string)null
//                                       select new
//                                       {
//                                           Taxonomy = hasRank8 ? rank1Name + ": " + rank2Name + ": " + rank3Name + ": " + rank4Name + ": " + rank5Name + ": " + rank6Name + ": " + rank7Name + ": " + rank8Name : "[no-rank-found]",
//                                           Name = isSpeciesRank ? rank8GenusSpeciesName : rank8Name,
//                                           RankPosition = "8",
//                                           RankName = rank8Name,
//                                           RankType = rank8Type,
//                                           ParentRankName = rank7Name,
//                                           Rank1 = s.Taxonomy.ElementAtOrDefault(0),
//                                           Rank2 = s.Taxonomy.ElementAtOrDefault(1),
//                                           Rank3 = s.Taxonomy.ElementAtOrDefault(2),
//                                           Rank4 = s.Taxonomy.ElementAtOrDefault(3),
//                                           Rank5 = s.Taxonomy.ElementAtOrDefault(4),
//                                           Rank6 = s.Taxonomy.ElementAtOrDefault(5),
//                                           Rank7 = s.Taxonomy.ElementAtOrDefault(6),
//                                           Rank8 = s.Taxonomy.ElementAtOrDefault(7),
//                                           s.Category,
//                                           s.CommonGroupNames,
//                                           s.CommonNames,
//                                           SpeciesIds = new[] { s.Id },
//                                           AllNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames,
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank7Name,
//                                                   rank8Name,
//                                                   rank8GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllScientificNames = new object[]
//                                               {
//                                                   rank1Name,
//                                                   rank2Name,
//                                                   rank3Name,
//                                                   rank4Name,
//                                                   rank5Name,
//                                                   rank6Name,
//                                                   rank7Name,
//                                                   rank8Name,
//                                                   rank8GenusSpeciesName,
//                                                   s.Synonym
//                                               },
//                                           AllCommonNames = new object[]
//                                               {
//                                                   s.CommonGroupNames,
//                                                   s.CommonNames
//                                               }
//                                       });

//            Reduce = results => from result in results
//                                where result.Taxonomy != "[no-rank-found]"
//                                group result by result.Taxonomy
//                                    into g
//                                    select new
//                                    {
//                                        Taxonomy = g.Key,
//                                        Name = g.Select(x => x.Name).FirstOrDefault(),
//                                        RankPosition = g.Select(x => x.RankPosition).FirstOrDefault(),
//                                        RankName = g.Select(x => x.RankName).FirstOrDefault(),
//                                        RankType = g.Select(x => x.RankType).FirstOrDefault(),
//                                        ParentRankName = g.Select(x => x.ParentRankName).FirstOrDefault(),
//                                        Rank1 = g.Select(x => x.Rank1).FirstOrDefault(),
//                                        Rank2 = g.Select(x => x.Rank2).FirstOrDefault(),
//                                        Rank3 = g.Select(x => x.Rank3).FirstOrDefault(),
//                                        Rank4 = g.Select(x => x.Rank4).FirstOrDefault(),
//                                        Rank5 = g.Select(x => x.Rank5).FirstOrDefault(),
//                                        Rank6 = g.Select(x => x.Rank6).FirstOrDefault(),
//                                        Rank7 = g.Select(x => x.Rank7).FirstOrDefault(),
//                                        Rank8 = g.Select(x => x.Rank8).FirstOrDefault(),
//                                        Category = g.Select(x => x.Category).FirstOrDefault(),
//                                        CommonGroupNames = g.SelectMany(x => x.CommonGroupNames),
//                                        CommonNames = g.SelectMany(x => x.CommonNames),
//                                        SpeciesIds = g.SelectMany(x => x.SpeciesIds),
//                                        AllNames = g.SelectMany(x => x.AllNames),
//                                        AllScientificNames = g.SelectMany(x => x.AllScientificNames),
//                                        AllCommonNames = g.SelectMany(x => x.AllCommonNames)
//                                    };

//            TransformResults = (database, results) =>
//                                from result in results
//                                select new
//                                {
//                                    result.Taxonomy,
//                                    result.Name,
//                                    result.RankPosition,
//                                    result.RankName,
//                                    result.RankType,
//                                    result.ParentRankName,
//                                    result.Rank1,
//                                    result.Rank2,
//                                    result.Rank3,
//                                    result.Rank4,
//                                    result.Rank5,
//                                    result.Rank6,
//                                    result.Rank7,
//                                    result.Rank8,
//                                    result.Category,
//                                    CommonGroupNames = result.CommonGroupNames.Distinct(),
//                                    CommonNames = result.CommonNames.Distinct(),
//                                    SpeciesIds = result.SpeciesIds.Distinct(),
//                                    AllNames = result.AllNames.Distinct(),
//                                    AllScientificNames = result.AllScientificNames.Distinct(),
//                                    AllCommonNames = result.AllCommonNames.Distinct(),
//                                    Species = database.Load<Species>(result.SpeciesIds)
//                                };

//            Store(x => x.Taxonomy, FieldStorage.Yes);
//            Store(x => x.Name, FieldStorage.Yes);
//            Store(x => x.RankPosition, FieldStorage.Yes);
//            Store(x => x.RankName, FieldStorage.Yes);
//            Store(x => x.RankType, FieldStorage.Yes);
//            Store(x => x.ParentRankName, FieldStorage.Yes);
//            Store(x => x.Rank1, FieldStorage.Yes);
//            Store(x => x.Rank2, FieldStorage.Yes);
//            Store(x => x.Rank3, FieldStorage.Yes);
//            Store(x => x.Rank4, FieldStorage.Yes);
//            Store(x => x.Rank5, FieldStorage.Yes);
//            Store(x => x.Rank6, FieldStorage.Yes);
//            Store(x => x.Rank7, FieldStorage.Yes);
//            Store(x => x.Rank8, FieldStorage.Yes);
//            Store(x => x.Category, FieldStorage.Yes);
//            Store(x => x.CommonGroupNames, FieldStorage.Yes);
//            Store(x => x.CommonNames, FieldStorage.Yes);
//            Store(x => x.SpeciesIds, FieldStorage.Yes);
//            Store(x => x.AllNames, FieldStorage.Yes);
//            Store(x => x.AllScientificNames, FieldStorage.Yes);
//            Store(x => x.AllCommonNames, FieldStorage.Yes);
//        }
//    }
//}

/////* Bowerbird V1 - Licensed under MIT 1.1 Public License

//// Developers: 
//// * Frank Radocaj : frank@radocaj.com
//// * Hamish Crittenden : hamish.crittenden@gmail.com

//// Project Manager: 
//// * Ken Walker : kwalker@museum.vic.gov.au

//// Funded by:
//// * Atlas of Living Australia

////*/

////using System;
////using System.Collections.Generic;
////using System.Linq;
////using Bowerbird.Core.DomainModels;
////using Raven.Abstractions.Indexing;
////using Raven.Client.Indexes;

////namespace Bowerbird.Core.Indexes
////{
////    public class All_CommonNameTaxa : AbstractMultiMapIndexCreationTask<All_CommonNameTaxa.Result>
////    {
////        public class Result
////        {
////            public string Taxonomy { get; set; }
////            public string Name { get; set; }
////            public string RankPosition { get; set; }
////            public string RankName { get; set; }
////            public string RankType { get; set; }
////            public string ParentRankName { get; set; }
////            public IDictionary<string, string> Rank1 { get; set; }
////            public IDictionary<string, string> Rank2 { get; set; }
////            public IDictionary<string, string> Rank3 { get; set; }
////            public IDictionary<string, string> Rank4 { get; set; }
////            public IDictionary<string, string> Rank5 { get; set; }
////            public IDictionary<string, string> Rank6 { get; set; }
////            public IDictionary<string, string> Rank7 { get; set; }
////            public IDictionary<string, string> Rank8 { get; set; }
////            public string Category { get; set; }
////            public string[] CommonGroupNames { get; set; }
////            public string[] CommonNames { get; set; }
////            public string[] SpeciesIds { get; set; }
////            public object[] AllNames { get; set; }
////            public object[] AllScientificNames { get; set; }
////            public object[] AllCommonNames { get; set; }
////            public IEnumerable<Species> Species { get; set; }
////        }

////        public All_CommonNameTaxa()
////        {
////            // Rank 1
////            AddMap<Species>(species => from s in species
////                                       select new
////                                           {
////                                               Taxonomy = s.Category,
////                                               Name = s.Category,
////                                               RankPosition = "1",
////                                               RankName = s.Category,
////                                               RankType = "category",
////                                               Rank1 = s.Category,
////                                               Rank2 = (string)null,
////                                               Rank3 = (string)null,
////                                               s.Category,
////                                               SpeciesIds = new [] { s.Id },
////                                               AllNames = new object[]
////                                                   {
////                                                       s.CommonGroupNames,
////                                                       s.CommonNames
////                                                   }
////                                           });

////            // Rank 2
////            AddMap<Species>(species => from s in species
////                                       from cgn in s.CommonGroupNames
////                                       select new
////                                       {
////                                           Taxonomy = s.Category + ": " + cgn,
////                                           Name = cgn,
////                                           RankPosition = "2",
////                                           RankName = cgn,
////                                           RankType = "common group name",
////                                           Rank1 = s.Category,
////                                           Rank2 = cgn,
////                                           Rank3 = (string)null,
////                                           s.Category,
////                                           SpeciesIds = new[] { s.Id },
////                                           AllNames = new object[]
////                                                   {
////                                                       s.CommonGroupNames,
////                                                       s.CommonNames
////                                                   }
////                                       });

////            // Rank 3
////            AddMap<Species>(species => from s in species
////                                       from cgn in s.CommonGroupNames
////                                       from cn in s.CommonNames
////                                       select new
////                                       {
////                                           Taxonomy = s.Category + ": " + cgn + ": " + cn,
////                                           Name = cn,
////                                           RankPosition = "3",
////                                           RankName = cn,
////                                           RankType = "common name",
////                                           Rank1 = s.Category,
////                                           Rank2 = cgn,
////                                           Rank3 = cn,
////                                           s.Category,
////                                           SpeciesIds = new[] { s.Id },
////                                           AllNames = new object[]
////                                                   {
////                                                       s.CommonGroupNames,
////                                                       s.CommonNames
////                                                   }
////                                       });

////            Reduce = results => from result in results
////                                where result.Taxonomy != "[no-rank-found]"
////                                group result by result.Taxonomy
////                                    into g
////                                    select new
////                                    {
////                                        Taxonomy = g.Key,
////                                        Name = g.Select(x => x.Name).FirstOrDefault(),
////                                        RankPosition = g.Select(x => x.RankPosition).FirstOrDefault(),
////                                        RankName = g.Select(x => x.RankName).FirstOrDefault(),
////                                        RankType = g.Select(x => x.RankType).FirstOrDefault(),
////                                        ParentRankName = g.Select(x => x.ParentRankName).FirstOrDefault(),
////                                        Rank1 = g.Select(x => x.Rank1).FirstOrDefault(),
////                                        Rank2 = g.Select(x => x.Rank2).FirstOrDefault(),
////                                        Rank3 = g.Select(x => x.Rank3).FirstOrDefault(),
////                                        Rank4 = g.Select(x => x.Rank4).FirstOrDefault(),
////                                        Rank5 = g.Select(x => x.Rank5).FirstOrDefault(),
////                                        Rank6 = g.Select(x => x.Rank6).FirstOrDefault(),
////                                        Rank7 = g.Select(x => x.Rank7).FirstOrDefault(),
////                                        Rank8 = g.Select(x => x.Rank8).FirstOrDefault(),
////                                        Category = g.Select(x => x.Category).FirstOrDefault(),
////                                        CommonGroupNames = g.SelectMany(x => x.CommonGroupNames),
////                                        CommonNames = g.SelectMany(x => x.CommonNames),
////                                        SpeciesIds = g.SelectMany(x => x.SpeciesIds),
////                                        AllNames = g.SelectMany(x => x.AllNames),
////                                        AllScientificNames = g.SelectMany(x => x.AllScientificNames),
////                                        AllCommonNames = g.SelectMany(x => x.AllCommonNames)
////                                    };

////            TransformResults = (database, results) =>
////                                from result in results
////                                select new
////                                {
////                                    result.Taxonomy,
////                                    result.Name,
////                                    result.RankPosition,
////                                    result.RankName,
////                                    result.RankType,
////                                    result.ParentRankName,
////                                    result.Rank1,
////                                    result.Rank2,
////                                    result.Rank3,
////                                    result.Rank4,
////                                    result.Rank5,
////                                    result.Rank6,
////                                    result.Rank7,
////                                    result.Rank8,
////                                    result.Category,
////                                    CommonGroupNames = result.CommonGroupNames.Distinct(),
////                                    CommonNames = result.CommonNames.Distinct(),
////                                    SpeciesIds = result.SpeciesIds.Distinct(),
////                                    AllNames = result.AllNames.Distinct(),
////                                    AllScientificNames = result.AllScientificNames.Distinct(),
////                                    AllCommonNames = result.AllCommonNames.Distinct(),
////                                    Species = database.Load<Species>(result.SpeciesIds)
////                                };

////            Store(x => x.Taxonomy, FieldStorage.Yes);
////            Store(x => x.Name, FieldStorage.Yes);
////            Store(x => x.RankPosition, FieldStorage.Yes);
////            Store(x => x.RankName, FieldStorage.Yes);
////            Store(x => x.RankType, FieldStorage.Yes);
////            Store(x => x.ParentRankName, FieldStorage.Yes);
////            Store(x => x.Rank1, FieldStorage.Yes);
////            Store(x => x.Rank2, FieldStorage.Yes);
////            Store(x => x.Rank3, FieldStorage.Yes);
////            Store(x => x.Rank4, FieldStorage.Yes);
////            Store(x => x.Rank5, FieldStorage.Yes);
////            Store(x => x.Rank6, FieldStorage.Yes);
////            Store(x => x.Rank7, FieldStorage.Yes);
////            Store(x => x.Rank8, FieldStorage.Yes);
////            Store(x => x.Category, FieldStorage.Yes);
////            Store(x => x.CommonGroupNames, FieldStorage.Yes);
////            Store(x => x.CommonNames, FieldStorage.Yes);
////            Store(x => x.SpeciesIds, FieldStorage.Yes);
////            Store(x => x.AllNames, FieldStorage.Yes);
////            Store(x => x.AllScientificNames, FieldStorage.Yes);
////            Store(x => x.AllCommonNames, FieldStorage.Yes);
////        }
////    }
////}