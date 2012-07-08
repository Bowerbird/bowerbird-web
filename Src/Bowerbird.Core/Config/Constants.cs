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

namespace Bowerbird.Core.Config
{
    public static class Constants
    {
        public static string EmailCookieName = "56277e138f774318ab152a84dad7adf9";
        public static string AppRootId = "bowerbird/approot";
    }

    /// <summary>
    /// Set an Avatar Type when creating new entity which requires a blank Avatar.
    /// Possibilities are Project, Team, Organisation and User
    /// </summary>
    public enum AvatarDefaultType
    {
        Project,
        Team,
        Organisation,
        User
    }

    public static class AvatarUris
    {
        public static string DefaultUser = "/img/default-user-avatar.jpg";
        public static string DefaultProject = "/img/default-project-avatar.jpg";
        public static string DefaultTeam = "/img/default-team-avatar.jpg";
        public static string DefaultOrganisation = "/img/default-organisation-avatar.jpg";

        public static string AvatarTypeUris(this AvatarDefaultType avatarType)
        {
            switch (avatarType)
            {
                case AvatarDefaultType.Project:
                    {
                        return DefaultProject;
                    }
                case AvatarDefaultType.Team:
                    {
                        return DefaultTeam;
                    }
                case AvatarDefaultType.Organisation:
                    {
                        return DefaultOrganisation;
                    }
                case AvatarDefaultType.User:
                    {
                        return DefaultUser;
                    }
                default:
                throw new ApplicationException("No Avatar Type Specified");
            }
        }
    }

    public static class ActivitySender
    {
        public static string Observation { get { return "observation"; }}
        public static string User { get { return "user"; } }
        public static string Post { get {return "post"; }}
        public static string ObservationNote { get {return "observationnote"; }}
        public static string Project { get {return "project"; }}
        public static string Team { get {return "team"; }}
        public static string Organisation { get {return "organisation"; }}
        public static string Comment { get {return "comment"; }}

        public static string AsActivitySender(this string noun)
        {
            switch(noun.ToLower())
            {
                case "observation": return Observation;
                case "user": return User;
                case "post": return Post;
                case "observationnote": return ObservationNote;
                case "project": return Project;
                case "team": return Team;
                case "organisation": return Organisation;
                case "comment": return Comment;
                default: return "unknown";
            }
        }
    }

    public static class ActivityAction
    {
        public static string Created { get { return "created"; } }
        public static string Updated { get { return "updated"; } }
        public static string Deleted { get { return "deleted"; } }
        public static string Added { get { return "added"; } }
        public static string Joined { get { return "joined"; } }
        public static string LoggedIn { get { return "loggedin"; } }
        public static string Flagged { get { return "flagged"; } }
    }

    public static class ActivityMessage
    {
        public static string CreatedAGroup { get { return "{0} created a {1} called {2}"; } }
        public static string UpdatedAGroup { get { return "{0} updated the {1} {2}"; } }
        public static string AddedAGroupToAGroup { get { return "{0} added a {1} titled {2} to the {3} {4}"; } }
        public static string AddedAContributionToAGroup { get { return "{0} added a {1} titled {2} in the {3} {4}"; } }
        public static string UpdatedAGroupContribution { get { return "{0} updated the {1} {2}"; } }
        public static string Commented { get { return "{0} said {1} about the {2} {3}"; } }
        public static string CreatedAnObservationNote { get { return "{0} added an observation note to the {1} observation"; } }
        public static string UpdatedAnObservationNote { get { return "{0} updated an observation note on the {1} observation"; } }
        public static string CreatedAnObservation { get { return "{0} created the {1} observation"; } }
        public static string UpdatedAnObservation { get { return "{0} updated the {1} observation"; } }
        public static string Joined { get { return "{0} joined Bowerbird"; } }
        public static string JoinedAGroup { get { return "{0} joined Bowerbird"; } }
        public static string LoggedIn { get { return "{0} logged in"; } }
        public static string FlaggedAnItem { get { return "{0} flagged an item"; } }
        public static string UpdatedTheirDetails { get { return "{0} updated their details"; } }
        public static string CreatedAWatchlist { get { return "{0} created a watchlist {1}"; } }
        public static string UpdatedTheirWatchlist { get { return "{0} updated their {1} watchlist"; } }
        public static string AddMemberToGroup { get { return "{0} joined the {1} {2}"; } }
    }

    public static class Form
    {
        public static string List = "List";
        public static string Index = "Index";
        public static string PublicIndex = "PublicIndex";
        public static string PrivateIndex = "PrivateIndex";
        public static string Activity = "Activity";
        public static string Observations = "Observations";
        public static string Posts = "Posts";
        public static string Post = "Post";
        public static string Members = "Members";
        public static string About = "About";
        public static string Create = "Create";
        public static string Update = "Update";
        public static string Delete = "Delete";
        public static string Login = "Login";
        public static string LogoutSuccess = "LogoutSuccess";
        public static string ChangePassword = "ChangePassword";
        public static string ResetPassword = "ResetPassword";
        public static string RequestPasswordReset = "RequestPasswordReset";
        public static string Register = "Register";
        public static string RequestPasswordResetSuccess = "RequestPasswordResetSuccess";
        public static string Following = "Following";
        public static string Followers = "Followers";
        public static string Sightings = "Sightings";
        public static string Teams = "Teams";
    }

    public static class MediaType
    {
        public static string Image = "image";
        public static string Document = "document";
        public static string Video = "video";
    }

    public static class Default
    {
        public static int PageStart = 1;
        public static int PageSize = 10;
        public static int PageMax = 100;
        public const string MovieWidth = "520";
        public const string MovieHeight = "390";
    }

}