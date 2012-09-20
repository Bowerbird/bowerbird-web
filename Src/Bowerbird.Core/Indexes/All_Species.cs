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
            
            public string SpeciesId { get; set; }
            //public DateTime CreatedOn { get; set; }
            //public string Category { get; set; }
            //public string CommonGroupName  { get; set; }
            //public IEnumerable<string> CommonNames { get; set; }
            //public string KingdomName { get; set; }
            //public string PhylumName { get; set; }
            //public string ClassName { get; set; }
            //public string OrderName { get; set; }
            //public string FamilyName { get; set; }
            //public string GenusName { get; set; }
            //public string SpeciesName { get; set; }
            public string QueryField { get; set; }

            public Species Species { get; set; }
        }

        public All_Species()
        {
            AddMap<Species>(species => from s in species
                                       select new
                                       {
                                           SpeciesId = s.Id,
                                           SpeciesCreatedOn = s.CreatedDateTime,
                                           QueryField = new object[]
                                               {
                                                   s.Category,
                                                   s.CommonGroupName,
                                                   s.CommonNames,
                                                   s.KingdomName,
                                                   s.PhylumName,
                                                   s.ClassName,
                                                   s.OrderName,
                                                   s.FamilyName,
                                                   s.GenusName,
                                                   s.SpeciesName,
                                               }
                                       });

            //Analyzers =
            //{
            //{ x => x.QueryField, "StandardAnalyzer"}
            //}

            //TransformResults = (database, results) =>
            //    from result in results
            //    select new
            //    {
            //        result.SpeciesId,
            //        //result.CommonGroupName,
            //        //result.CommonNames,
            //        //result.KingdomName,
            //        //result.PhylumName,
            //        //result.ClassName,
            //        //result.OrderName,
            //        //result.FamilyName,
            //        //result.GenusName,
            //        //result.SpeciesName,
            //        //result.CreatedOn,
            //        Species = database.Load<Species>(result.SpeciesId)
            //    };

            Store(x => x.SpeciesId, FieldStorage.Yes);
            Store(x => x.QueryField, FieldStorage.Yes);

            Index(x => x.QueryField, FieldIndexing.Analyzed);

            //Analyze(x => x.QueryField, "StandardAnalyzer");

            //Analyze(x => x.QueryField, );
            //Store(x => x.CommonGroupName, FieldStorage.Yes);
            //Store(x => x.CommonNames, FieldStorage.Yes);
            //Store(x => x.KingdomName, FieldStorage.Yes);
            //Store(x => x.PhylumName, FieldStorage.Yes);
            //Store(x => x.ClassName, FieldStorage.Yes);
            //Store(x => x.OrderName, FieldStorage.Yes);
            //Store(x => x.FamilyName, FieldStorage.Yes);
            //Store(x => x.GenusName, FieldStorage.Yes);
            //Store(x => x.SpeciesName, FieldStorage.Yes);
            //Store(x => x.CreatedOn, FieldStorage.Yes);
        }
    }
}