using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Factories
{
    public class BrowseItemFactory : IBrowseItemFactory
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(object item, IEnumerable<string> groups, string contributionType, User groupUser, DateTime groupCreatedDateTime, string description)
        {
            return new 
            {
                createdDateTime = groupCreatedDateTime,
                createdDateTimeDescription = MakeCreatedDateTimeDescription(groupCreatedDateTime),
                type = contributionType.ToLower(),
                user = new 
                {
                    id = groupUser.Id,
                    lastLoggedIn = groupUser.LastLoggedIn,
                    name = groupUser.FirstName + " " + groupUser.LastName,
                    avatar = new
                    {
                        altTag = groupUser.FirstName + " " + groupUser.LastName,
                        urlToImage = groupUser.Avatar != null ? "" : AvatarUris.DefaultUser
                    }
                },
                item,
                description,
                groups
            };
        }

        private string MakeCreatedDateTimeDescription(DateTime dateTime)
        {
            var diff = DateTime.Now.Subtract(dateTime);

            if (diff > new TimeSpan(365, 0, 0, 0)) // Year
            {
                return "more than a year ago";
            }
            else if (diff > new TimeSpan(30, 0, 0, 0)) // Month
            {
                var months = (diff.Days / 30);
                return string.Format("{0} month{1} ago", months, months > 1 ? "s" : string.Empty);
            }
            else if (diff > new TimeSpan(1, 0, 0, 0)) // Day
            {
                return string.Format("{0} day{1} ago", diff.Days, diff.Days > 1 ? "s" : string.Empty);
            }
            else if (diff > new TimeSpan(1, 0, 0)) // Hour
            {
                return string.Format("{0} hour{1} ago", diff.Hours, diff.Hours > 1 ? "s" : string.Empty);
            }
            else if (diff > new TimeSpan(0, 1, 0)) // Minute
            {
                return string.Format("{0} minute{1} ago", diff.Minutes, diff.Minutes > 1 ? "s" : string.Empty);
            }
            else // Second
            {
                return "just now";
            }
        }

        #endregion

    }
}
