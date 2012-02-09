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
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedContributionDomainModelReference : ValueObject
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedContributionDomainModelReference(Contribution contribution)
        {
            Check.RequireNotNull(contribution, "contribution");

            return new DenormalisedContributionDomainModelReference
            {
                Id = contribution.Id,
                UserId = contribution.User.Id,
                CreatedDateTime = contribution.CreatedOn
            };
        }

        #endregion
    }
}