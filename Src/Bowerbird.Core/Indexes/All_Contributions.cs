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
using Raven.Client.Indexes;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Indexes
{
    public class All_Contributions : AbstractMultiMapIndexCreationTask<All_Contributions.Result>
    {
        public class Result
        {
            public string ContributionId { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }
        }

        public All_Contributions()
        {

            AddMap<Post>(posts => from post in posts
                                  select new
                                  {
                                      ContributionId = post.Id,
                                      CreatedDateTime = post.CreatedOn,
                                      UserId = post.User.Id,
                                  });

            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    CreatedDateTime = observation.CreatedOn,
                                                    UserId = observation.User.Id
                                                });

            AddMap<ObservationNote>(observationNotes => from observationNote in observationNotes
                                                        select new
                                                        {
                                                            ContributionId = observationNote.Id,
                                                            CreatedDateTime = observationNote.CreatedOn,
                                                            UserId = observationNote.User.Id
                                                        });
        }
    }
}
