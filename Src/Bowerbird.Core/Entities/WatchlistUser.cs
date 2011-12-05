using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class WatchlistUser : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected WatchlistUser()
            : base()
        {
        }

        public WatchlistUser(
            Watchlist watchlist,
            User user 
            )
        {
            Contract.RequireNotNull(watchlist, "watchlist");
            Contract.RequireNotNull(user, "user");

            Watchlist = watchlist;
            User = user;
        }

        #endregion

        #region Properties

        public virtual Watchlist Watchlist { get; set; }

        [Required]
        public virtual User User { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}