/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class Watchlist : DomainModel, IOwnable
    {
        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Persistence required constructor
        /// </summary>
        public Watchlist()
            : base()
        {
        }

        public Watchlist(
            User createdByUser,
            string name,
            string querystringJson)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(querystringJson, "querystringJson");

            User = createdByUser;

            SetWatchlistDetails(
                name,
                querystringJson);

            ApplyEvent(new DomainModelUpdatedEvent<Watchlist>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public string Name { get; private set; }

        public string QuerystringJson { get; private set; }

        IEnumerable<string> IOwnable.Groups
        {
            get { return new List<string>(); }
        }
        
        #endregion

        #region Methods

        private void SetWatchlistDetails(string name, string querystringJson)
        {
            Name = name;
            QuerystringJson = querystringJson;
        }

        public void UpdateDetails( User updatedByUser, string name, string querystringJson)
        {
            SetWatchlistDetails(name, querystringJson);

            ApplyEvent(new DomainModelUpdatedEvent<Watchlist>(this, updatedByUser, this));
        }

        #endregion

    }
}