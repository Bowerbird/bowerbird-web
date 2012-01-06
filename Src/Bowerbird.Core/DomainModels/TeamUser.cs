using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class TeamUser : UserCreatedEntityBase
    {
        
        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected TeamUser() : base() { }

        public TeamUser(
            User user,
            Team team,
            User editor,
            long groupUserKey
           )
            : base(editor)
        {
            Contract.RequireNotNull(team, "team");
            Contract.RequireNotNull(user, "user");
            Contract.RequireGreaterThanZero(groupUserKey, "groupUserKey");

            Team = team;
            User = user;
            TeamUserKey = groupUserKey;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Team Team { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual long TeamUserKey { get; set; }
        
        #endregion

        #region Methods

        #endregion

    }
}