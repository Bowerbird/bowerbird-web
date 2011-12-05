using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Entities
{
    public class Watchlist : Entity
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
            string name,
            string querystringJson,
            User createdByUser)
            //: base(createdByUser)
        {
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(querystringJson, "querystringJson");

            Name = name;
            QuerystringJson = querystringJson;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string QuerystringJson { get; set; }
        
        #endregion

        #region Methods

        #endregion

    }
}