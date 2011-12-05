using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;
using SharpArch.Domain.DomainModel;

namespace Bowerbird.Core.Entities
{
    public class OrganisationUser : UserCreatedEntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected OrganisationUser()
            : base()
        {
        }

        public OrganisationUser(
            long organisationUserKey,
            Organisation organisation,
            User user,
            User createdByUser
            )
            : base(createdByUser)
        {
            Contract.RequireNotNull(organisation, "organisation");
            Contract.RequireNotNull(user, "user");
            Contract.RequireGreaterThanZero(organisationUserKey, "organisationUserKey");

            OrganisationUserKey = organisationUserKey;
            Organisation = organisation;
            User = user;
        }

        #endregion

        #region Properties

        [DomainSignature]
        [Required]
        public virtual long OrganisationUserKey { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual Organisation Organisation { get; set; }

        public virtual bool IsAdministrator { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}