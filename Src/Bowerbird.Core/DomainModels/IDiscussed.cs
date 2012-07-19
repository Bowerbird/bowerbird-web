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

namespace Bowerbird.Core.DomainModels
{
    public interface IDiscussed
    {
        IContribution AddComment(string message, User createdByUser, DateTime createdDateTime, string contributionId);

        IContribution AddThreadedComment(string message, User createdByUser, DateTime createdDateTime, Comment inReplyToComment, string contributionId);

        IContribution RemoveComment(string commentId);

        IContribution UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime);
    }
}