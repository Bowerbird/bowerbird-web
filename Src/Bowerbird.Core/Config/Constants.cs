/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Config
{
    public static class Constants
    {
        public static string EmailCookieName = "56277e138f774318ab152a84dad7adf9";
    }

    public static class Nouns
    {
        public static string Observation { get { return "observation"; }}
        public static string User { get { return "user"; } }
        public static string Post { get {return "post"; }}
        public static string ObservationNote { get {return "observationnote"; }}
        public static string Project { get {return "project"; }}
        public static string Team { get {return "team"; }}
        public static string Organisation { get {return "organisation"; }}
        public static string Comment { get {return "comment"; }}

        public static string ResolveAsNoun(this string noun)
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

    public static class Verbs
    {
        public static string Created { get { return "created"; } }
        public static string Updated { get { return "updated"; } }
        public static string Deleted { get { return "deleted"; } }
        public static string Added { get { return "added"; } }
        public static string Joined { get { return "joined"; } }
        public static string LoggedIn { get { return "loggedin"; } }
        public static string Flagged { get { return "flagged"; } }
    }

    public static class ActivityMessages
    {
        public static string CreatedAGroup { get { return "{0} created a {1} called {2}"; } }
        public static string UpdatedAGroup { get { return "{0} updated the {1} {2}"; } }
        public static string AddedAGroupToAGroup { get { return "{0} added a {1} titled {2} to the {3} {4}"; } }
        public static string AddedAContributionToAGroup { get { return "{0} added a {1} titled {2} in the {3} {4}"; } }
        public static string UpdatedAGroupContribution { get { return "{0} updated the {1} {2}"; } }
        public static string Commented { get { return "{0} said {1} about the {2} {3}"; } }
        public static string CreatedAnObservationNote { get { return "{0} added to the {1} observation"; } }
        public static string UpdatedAnObservationNote { get { return "{0} updated a {1}"; } }
        public static string CreatedAnObservation { get { return "{0} created the {1} observation"; } }
        public static string UpdatedAnObservation { get { return "{0} updated the {1} observation"; } }
        public static string Joined { get { return "{0} joined Bowerbird"; } }
        public static string JoinedAGroup { get { return "{0} joined Bowerbird"; } }
        public static string LoggedIn { get { return "{0} logged in"; } }
        public static string FlaggedAnItem { get { return "{0} flagged an item"; } }
        public static string UpdatedTheirDetails { get { return "{0} updated their details"; } }
        public static string CreatedAWatchlist { get { return "{0} created a watchlist {1}"; } }
        public static string UpdatedTheirWatchlist { get { return "{0} updated their {1} watchlist"; } }
    }
}