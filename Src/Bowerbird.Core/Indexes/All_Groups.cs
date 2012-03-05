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
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<Group>
    {
        public All_Groups()
        {
            AddMap<Organisation>(organisations => from organisation in organisations
                                                  select new
                                                             {
                                                                organisation.Name,
                                                                organisation.Description
                                                             });

            AddMap<Team>(teams => from team in teams
                                                select new
                                                {
                                                    team.Name,
                                                    team.Description
                                                });


            AddMap<Project>(projects => from project in projects
                                  select new
                                  {
                                      project.Name,
                                      project.Description
                                  });
        }
    }
}
