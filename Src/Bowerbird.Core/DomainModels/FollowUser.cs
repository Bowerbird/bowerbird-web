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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class FollowUser : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        public FollowUser(
            User userToFollow,
            User follower,
            DateTime createdDateTime
            )
        {
            Check.RequireNotNull(userToFollow, "userToFollow");
            Check.RequireNotNull(follower, "follower");

            UserToFollow = userToFollow;
            Follower = follower;
            CreatedDateTime = createdDateTime;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference UserToFollow { get; set; }

        public DenormalisedUserReference Follower { get; set; }

        public DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}