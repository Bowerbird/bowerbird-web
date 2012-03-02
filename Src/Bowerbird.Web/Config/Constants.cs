/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Config
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
        public static string AddedA { get { return "added a"; } }
        public static string AddedTo { get { return "added to"; } }
        public static string Joined { get { return "joined Bowerbird"; } }
        public static string LoggedIn { get { return "logged in"; } }
        public static string Flagged { get { return "flagged an item"; } }
    }
}