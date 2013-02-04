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

namespace Bowerbird.Core.Services
{
    public interface IDateTimeZoneService
    {

        string GetOffsetFromUtcNow(string timezone);

        IDictionary<string, string> GetTimeZones(string countryCode);

        DateTime ExtractDateTimeFromExif(string dateTimeExif, string timezone);

    }
}