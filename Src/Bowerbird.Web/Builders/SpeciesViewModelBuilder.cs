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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.Builders
{
    public class SpeciesViewModelBuilder : ISpeciesViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SpeciesViewModelBuilder(
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        //public object BuildSpecies(string speciesId)
        //{
        //    Check.RequireNotNull(speciesId, "speciesId");

        //    return MakeSpecies(_documentSession.Load<Species>(speciesId));
        //}

        public object BuildSpeciesList(SpeciesQueryInput speciesQueryInput, PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var field = string.IsNullOrWhiteSpace(speciesQueryInput.Field) ? string.Empty : speciesQueryInput.Field;
            var queryText = string.IsNullOrWhiteSpace(speciesQueryInput.Query) ? string.Empty : speciesQueryInput.Query;
            var category = string.IsNullOrWhiteSpace(speciesQueryInput.Category) ? string.Empty : speciesQueryInput.Category;

            if (field.ToLower() == "allranks")
            {
                // Eg: query=Animalia: Chordata: Amphibia&field=rankall to get all browsable ranks for each rank level of the specified taxonomic classification
                var ranks = System.Web.HttpUtility.HtmlDecode(queryText).Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                var queryResults = new List<object>();

                queryResults.Add(
                    _documentSession
                    .Advanced
                    .LuceneQuery<All_Species.Result, All_Species>()
                    .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms")
                    .WhereEquals("RankPosition", 1)
                    .OrderBy(x => x.Name)
                    .Take(50)
                    .ToList()
                    .Select(x => MakeSpecies(x, string.Empty)));

                for(var rankIndex = 1; rankIndex < ranks.Count(); rankIndex++)
                {
                    queryResults.Add(
                        _documentSession
                        .Advanced
                        .LuceneQuery<All_Species.Result, All_Species>()
                        .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms")
                        .WhereEquals("ParentRankName", ranks.ElementAt(rankIndex - 1))
                        .AndAlso()
                        .WhereEquals("RankPosition", rankIndex + 1)
                        .OrderBy(x => x.Name)
                        .Take(50)
                        .ToList()
                        .Select(x => MakeSpecies(x, string.Empty)));
                }

                return queryResults;
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Species.Result, All_Species>()
                .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms")
                .Statistics(out stats);

            if (field.ToLower() == "taxonomy")
            {
                query.WhereEquals("Taxonomy", queryText);
            }
            else if(field.ToLower() == "scientific")
            {
                query.WhereStartsWith("AllScientificNames", queryText);
            }
            else if(field.ToLower() == "common")
            {
                query.WhereStartsWith("AllCommonNames", queryText);
            }
            else if (field.ToLower() == "rankposition")
            {
                // Eg: query=1&field=rankposition to get all rank values in position 1
                query.WhereEquals("RankPosition", queryText);
            }
            else if (field.ToLower() == "ranktype")
            {
                // Eg: query=kingdom&field=ranktype to get all kingdoms
                query.WhereEquals("RankType", queryText);
            }
            else if(field.ToLower() == "rank2" ||
                field.ToLower() == "rank3" ||
                field.ToLower() == "rank4" ||
                field.ToLower() == "rank5" ||
                field.ToLower() == "rank6" ||
                field.ToLower() == "rank7" ||
                field.ToLower() == "rank8")
            {
                // Eg: query=animalia&field=rank2 to get all ranks in rank 2 that have a parent rank named "animalia"
                query
                    .WhereEquals("ParentRankName", queryText)
                    .AndAlso()
                    .WhereEquals("RankPosition", field.ToLower().Replace("rank", string.Empty));
            }
            else
            {
                query.WhereStartsWith("AllNames", queryText);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                // Additionally restrict to a single category
                query.AndAlso().WhereEquals("Category", category);
            }

            return query
                .OrderBy(x => x.Name)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.GetPageSize())
                .ToList()
                .Select(x => MakeSpecies(x, queryText));
        }

        private static object MakeSpecies(All_Species.Result result, string query)
        {
            return new
            {
                result.Taxonomy,
                result.Name,
                result.RankPosition,
                result.RankName,
                result.RankType,
                result.ParentRankName,
                result.Ranks,
                result.Category,
                result.SpeciesCount,
                CommonGroupNames = result.CommonGroupNames.Where(x => x.ToLower().StartsWith(query.ToLower())),
                CommonNames = result.CommonNames.Where(x => x.ToLower().StartsWith(query.ToLower())),
                Synonyms = result.Synonyms.Where(x => x.ToLower().StartsWith(query.ToLower()))
            };
        }

        //private static object MakeSpecies(Species species)
        //{
        //    return new
        //    {
        //        species.Id,
        //        species.CommonGroupNames,
        //        species.CommonNames,
        //        species.KingdomName,
        //        species.PhylumName,
        //        species.ClassName,
        //        species.OrderName,
        //        species.FamilyName,
        //        species.GenusName,
        //        species.SpeciesName
        //    };
        //}

        #endregion
    }
}