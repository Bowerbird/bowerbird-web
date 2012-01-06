using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class OrganisationPost : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected OrganisationPost()
            : base()
        {
        }

        public OrganisationPost(
            Organisation organisation,
            Post post,
            User createdByUser
            )
        {
            Contract.RequireNotNull(organisation, "organisation");
            Contract.RequireNotNull(post, "post");
            Contract.RequireNotNull(createdByUser, "createdByUser");

            Organisation = organisation;
            Post = post;
            CreatedByUser = createdByUser;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Organisation Organisation { get; set; }

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