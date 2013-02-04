using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bowerbird.Core.Services;
using NodaTime;
using Bowerbird.Web.Properties;

namespace Bowerbird.Web.Services
{
    public class DateTimeZoneService : IDateTimeZoneService
    {

        #region Members

        private static volatile ILookup<string, string> _zones;
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string GetOffsetFromUtcNow(string timezone)
        {
            return DateTimeZoneProviders.Tzdb[timezone].GetOffsetFromUtc(new Instant()).ToString();
        }

        /// <summary>
        /// Returns a list of valid timezones as a dictionary, where the key is the timezone id, and the value can be used for display.
        /// </summary>
        /// <param name="countryCode">The two-letter country code to get timezones for.  Returns all timezones if null or empty.</param>
        public IDictionary<string, string> GetTimeZones(string countryCode)
        {
            var timeZones = string.IsNullOrEmpty(countryCode)
                                ? Zones.SelectMany(x => x)
                                : Zones[countryCode.ToUpper()];

            var now = Instant.FromDateTimeUtc(DateTime.UtcNow);
            var tzdb = DateTimeZoneProviders.Tzdb;

            return (from id in timeZones
                       let tz = tzdb[id]
                       let offset = tz.GetZoneInterval(now).StandardOffset
                       orderby offset, id
                       select new
                       {
                           Key = id,
                           Value = string.Format("{0} ({1})", id.Replace("_", " "), offset.ToString("+HH:mm", null))
                       }).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// EXIF DateTime is stored as a string - "yyyy:MM:dd hh:mm:ss" in 24 hour format.
        /// 
        /// Since EXIF datetime does not store timezone info, making the time ambiguous, we grab the user's specified timezone
        /// and assume the image was taken in that timezone.
        /// 
        /// If we don't have a parseable datetime, we set the time to now.
        /// </summary>
        public DateTime ExtractDateTimeFromExif(string dateTimeExif, string timezone)
        {
            var dateTimeStringComponents = dateTimeExif.Split(new[] { ':', ' ' });

            if (dateTimeExif != string.Empty && dateTimeStringComponents.Count() == 6)
            {
                var dateTimeIntComponents = new int[dateTimeStringComponents.Count()];

                for (var i = 0; i < dateTimeStringComponents.Length; i++)
                {
                    int convertedSegment;
                    if (Int32.TryParse(dateTimeStringComponents[i], out convertedSegment))
                    {
                        dateTimeIntComponents[i] = convertedSegment;
                    }
                }

                // Get data into a local time object (no time zone specified)
                var localDateTime = new LocalDateTime(
                        dateTimeIntComponents[0], // year
                        dateTimeIntComponents[1], // month
                        dateTimeIntComponents[2], // day
                        dateTimeIntComponents[3], // hour
                        dateTimeIntComponents[4], // minute
                        dateTimeIntComponents[5], // second
                        CalendarSystem.Iso);

                // Put the local date time into a timezone
                var zonedDateTime = localDateTime.InZoneLeniently(DateTimeZoneProviders.Tzdb[timezone]);

                // Get the UTC date time of the given local date time in the given time zone
                return zonedDateTime.WithZone(DateTimeZone.Utc).ToDateTimeUtc();
            }

            return DateTime.UtcNow;
        }

        private static ILookup<string, string> Zones
        {
            get
            {
                if (_zones != null)
                    return _zones;

                lock (SyncRoot)
                {
                    if (_zones == null)
                        _zones = ReadAndParseTimeZones().ToLookup(x => x.Key, x => x.Value);
                }

                return _zones;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> ReadAndParseTimeZones()
        {
            // Simple parser for the zones data, which is embedded as a string resource.
            // The data is sourced from the zone.tab file from the offical tz database.
            // TODO: When NodaTime embeds this file, switch to their copy so we don't have to maintain it.
            using (var reader = new StringReader(Resources.Zones))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // ignore comments and blank lines
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                        continue;

                    var data = line.Split('\t');
                    var key = data[0];
                    var value = data[2];
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

        #endregion  
 
    }
}
