﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    #endregion

    public class SightingNoteCreateCommandHandler : ICommandHandler<SightingNoteCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SightingNoteCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(SightingNoteCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            Sighting sighting;

            if (!string.IsNullOrWhiteSpace(command.SightingId))
            {
                sighting = _documentSession.Load<dynamic>(command.SightingId) as Sighting;
            } 
            else 
            {
                sighting = _documentSession
                    .Query<Observation>()
                    //.Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.Key == command.SightingKey).First();
            }

            Identification identification = null;

            if (!string.IsNullOrWhiteSpace(command.Taxonomy))
            {
                if (command.IsCustomIdentification)
                {
                    identification = new Identification(
                        true,
                        command.Category,
                        command.Kingdom,
                        command.Phylum,
                        command.Class,
                        command.Order,
                        command.Family,
                        command.Genus,
                        command.Species,
                        command.Subspecies,
                        command.CommonGroupNames,
                        command.CommonNames,
                        command.Synonyms);
                }
                else
                {
                    var result = _documentSession
                        .Advanced
                        .LuceneQuery<All_Species.Result, All_Species>()
                        .SelectFields<All_Species.Result>("Ranks", "Category", "CommonGroupNames", "CommonNames",
                                                            "Synonyms")
                        .WhereEquals("Taxonomy", command.Taxonomy)
                        .First();

                    identification = new Identification(
                        false,
                        result.Category,
                        GetRankName(result.Ranks, "kingdom"),
                        GetRankName(result.Ranks, "phylum"),
                        GetRankName(result.Ranks, "class"),
                        GetRankName(result.Ranks, "order"),
                        GetRankName(result.Ranks, "family"),
                        GetRankName(result.Ranks, "genus"),
                        GetRankName(result.Ranks, "species"),
                        GetRankName(result.Ranks, "subspecies"),
                        result.CommonGroupNames ?? new string[] {},
                        result.CommonNames ?? new string[] {},
                        result.Synonyms ?? new string[] {});
                }
            }

            sighting.AddNote(
                identification,
                command.Tags.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()),
                command.Descriptions,
                DateTime.UtcNow.AddSeconds(1), // Add a second to avoid notes being published before its parent sighting
                _documentSession.Load<User>(command.UserId));

            _documentSession.Store(sighting);
            _documentSession.SaveChanges();
        }

        private string GetRankName(IDictionary<string, string>[] ranks, string rankType)
        {
            if(ranks.Any(x => x["Type"] == rankType))
            {
                return ranks.First(x => x["Type"] == rankType)["Name"] ?? string.Empty;
            }
            return string.Empty;
        }

        #endregion

    }
}