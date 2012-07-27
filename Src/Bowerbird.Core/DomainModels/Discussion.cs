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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class Discussion : CommentBase
    {
        #region Members

        #endregion

        #region Constructors

        public Discussion()
            : base()
        {
            Id = string.Empty;
            SequentialId = 1;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public CommentNew UpdateDetails(string id, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Check.RequireNotNull(modifiedByUser, "modifiedByUser");

            foreach (var childComment in Comments)
            {
                var comment = childComment.UpdateDetails(id, message, modifiedByUser, modifiedDateTime);

                if (comment != null)
                {
                    return comment;
                }
            }

            return null;
        }

        #endregion
    }
}