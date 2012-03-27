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

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public All_Groups()
        {
            AddMap<Organisation>(organisations => from organisation in organisations
                                                  select new
                                                             { 
                                                                organisation.Id,
                                                                organisation.Name,
                                                                organisation.Description,
                                                             });

            AddMap<Team>(teams => from team in teams
                                                select new
                                                {
                                                    team.Id,
                                                    team.Name,
                                                    team.Description
                                                });


            AddMap<Project>(projects => from project in projects
                                  select new
                                  {
                                      project.Id,
                                      project.Name,
                                      project.Description
                                  });
        }
    }
}