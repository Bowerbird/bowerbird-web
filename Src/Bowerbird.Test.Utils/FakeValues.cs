using System;
using System.Collections.Generic;

namespace Bowerbird.Test.Utils
{
    public static class FakeValues
    {
        /// <summary>
        /// value: 100
        /// </summary>
        public static int Number { get { return 100; } }

        /// <summary>
        /// value: 123
        /// </summary>
        public static long KeyLong { get { return 123L; } }

        /// <summary>
        /// value: "abc"
        /// </summary>
        public static string KeyString { get { return "abc"; } }

        /// <summary>
        /// value: "moron.txt"
        /// </summary>
        public static string Filename { get { return "moron.txt"; } }

        /// <summary>
        /// value: "first name"
        /// </summary>
        public static string FirstName { get { return "first name"; } }

        /// <summary>
        /// value: "last name"
        /// </summary>
        public static string LastName { get { return "last name"; } }

        /// <summary>
        /// value: "username"
        /// </summary>
        public static string UserName { get { return "username"; } }

        /// <summary>
        /// value: "fake name"
        /// </summary>
        public static string Name { get { return "fake name"; } }

        /// <summary>
        /// value: "fake screen name"
        /// </summary>
        public static string ScreenName { get { return "fake screen name"; } }

        /// <summary>
        /// value: "password"
        /// </summary>
        public static string Password { get { return "password"; } }

        /// <summary>
        /// value: "padil@padil.gov.au"
        /// </summary>
        public static string Email { get { return "padil@padil.gov.au"; } }

        /// <summary>
        /// value: "padil.gov.au"
        /// </summary>
        public static string InvalidEmail { get { return "padil.gov.au"; } }

        /// <summary>
        /// value: "fake description"
        /// </summary>
        public static string Description { get { return "fake description"; } }

        /// <summary>
        /// value: "faketag1, faketag2"
        /// </summary>
        public static string Tags { get { return "faketag1, faketag2"; } }

        /// <summary>
        /// value: "fake message"
        /// </summary>
        public static string Message { get { return "fake message"; } }

        /// <summary>
        /// value: "fake summary"
        /// </summary>
        public static string Summary { get { return "fake summary"; } }

        /// <summary>
        /// value: 21/01/2011 12:56:45
        /// </summary>
        public static DateTime CreatedDateTime { get { return new DateTime(2011, 1, 21, 12, 56, 45); } }

        /// <summary>
        /// value: 27/06/2011 08:56:45
        /// </summary>
        public static DateTime ModifiedDateTime { get { return new DateTime(2011, 6, 27, 08, 56, 45); } }

        /// <summary>
        /// value: 123456
        /// </summary>
        public static int UserId { get { return 123546; } }

        /// <summary>
        /// value: true
        /// </summary>
        public static bool IsTrue { get { return true; } }

        /// <summary>
        /// value: false
        /// </summary>
        public static bool IsFalse { get { return false; } }

        /// <summary>
        /// value: "fake-search-index-key"
        /// </summary>
        public static string SearchIndexKey { get { return "fake-search-index-key"; } }

        /// <summary>
        /// value: "fake common name"
        /// </summary>
        public static string CommonName { get { return "fake common name"; } }

        /// <summary>
        /// value: "fake scientific name"
        /// </summary>
        public static string ScientificName { get { return "fake scientific name"; } }

        /// <summary>
        /// value: "fake taxonomy"
        /// </summary>
        public static string Taxonomy { get { return "fake taxonomy"; } }

        /// <summary>
        /// value: "35.158"
        /// </summary>
        public static string Latitude { get { return "35.158"; } }

        /// <summary>
        /// value: "-26.478598"
        /// </summary>
        public static string Longitude { get { return "-26.478598"; } }

        /// <summary>
        /// value: "fake street, fake suburb, fake state"
        /// </summary>
        public static string Address { get { return "fake street, fake suburb, fake state"; } }

        /// <summary>
        /// value: "Jpeg"
        /// </summary>
        public static string FileFormat { get { return "Jpeg"; } }

        /// <summary>
        /// value: {"1","2","3","4"}
        /// </summary>
        public static List<string> StringList { get { return new List<string>() { "1", "2", "3", "4" }; } }

        /// <summary>
        /// value: {1,2,3,4}
        /// </summary>
        public static IList<long> LongList { get { return new List<long>() { 1, 2, 3, 4 }; } }

        /// <summary>
        /// value: 1374
        /// </summary>
        public static int PasswordSalt { get { return 1374; } }

        /// <summary>
        /// value: "KJSHD0087GHUJDG8"
        /// </summary>
        public static string HashedPassword { get { return "KJSHD0087GHUJDG8"; } }

        /// <summary>
        /// value: 11/11/2011 11:11:11
        /// </summary>
        public static DateTime LastLoggedIn { get { return new DateTime(2011, 11, 11, 11, 11, 11); } }

        /// <summary>
        /// value: "fake comment"
        /// </summary>
        public static string Comment { get { return "fake comment"; } }

        /// <summary>
        /// value: "fake title"
        /// </summary>
        public static string Title { get { return "fake title"; } }

        /// <summary>
        /// value: "fake subject"
        /// </summary>
        public static string Subject { get { return "fake subject"; } }

        /// <summary>
        /// value: "fake mission statement"
        /// </summary>
        public static string MissionStatement { get { return "fake mission statement"; } }

        /// <summary>
        /// value: "{ query: 'fakequery' }"
        /// </summary>
        public static string QuerystringJson { get { return "{ query: &quot;fakequery&quot; }"; } }

        /// <summary>
        /// value: "www.fakewebsite.com"
        /// </summary>
        public static string Website { get { return "www.fakewebsite.com"; } }

        /// <summary>
        /// value: "invalidpassword"
        /// </summary>
        public static string InvalidPassword { get { return "invalidpassword"; } }

        /// <summary>
        /// value: "fake category"
        /// </summary>
        public static string Category { get { return "fake category"; } }

        /// <summary>
        /// value: "fake activity"
        /// </summary>
        public static string ActivityType { get { return "fake activity"; } }

        /// <summary>
        /// value: 1
        /// </summary>
        public static int Page { get { return 1; } }
        
        /// <summary>
        /// value: 10
        /// </summary>
        public static int PageSize { get { return 10; } }

        /// <summary>
        /// value: new object()
        /// </summary>
        public static object Object { get { return new object(); } }
    }
}