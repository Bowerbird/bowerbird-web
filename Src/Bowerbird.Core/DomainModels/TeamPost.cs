using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class TeamPost : EntityBase
    {

        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected TeamPost() : base() { }

        public TeamPost(
            Team group,
            Post post,
            User user
            )
            : base()
        {
            Contract.RequireNotNull(group, "group");
            Contract.RequireNotNull(post, "post");
            Contract.RequireNotNull(user, "user");

            Team = group;
            Post = post;
            CreatedByUser = user;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Team Team { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}