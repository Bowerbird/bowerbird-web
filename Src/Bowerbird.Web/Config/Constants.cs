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
    }

    public static class Adjectives
    {
        public static string Created { get { return "created"; } }
        public static string Updated { get { return "updated"; } }
        public static string Deleted { get { return "deleted"; } }
        public static string Joined { get { return "joined"; } }
        public static string Flagged { get { return "flagged"; } }
    }
}