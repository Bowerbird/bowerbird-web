/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Extensions
{
    public static class GroupExtensions
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods
				
        public static string GroupType(this string groupId)
        {
            var groupType = groupId.Split('/')[0].ToLower();

            switch (groupType)
            {
                case "organisations":
                    {
                        return "Organisation";
                    }
                case "teams":
                    {
                        return "Team";
                    }
                case "projects":
                    {
                        return "Project";
                    }
                case "userproject":
                    {
                        return "User Project";
                    }
                case "approot":
                    {
                        return "Application";
                    }
                default:
                    return "Unknown Group Type";
            }
        }

        #endregion
    }
}