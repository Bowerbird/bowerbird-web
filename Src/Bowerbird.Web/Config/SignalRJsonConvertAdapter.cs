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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SignalR;

namespace Bowerbird.Web.Config
{
    public class SignalRJsonConvertAdapter : IJsonSerializer
    {
        public string Stringify(object obj)
        {
            string stuff = JsonConvert.SerializeObject(
                obj
                //Formatting.None, 
                //new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()}
                );

            return stuff;
        }

        public object Parse(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public object Parse(string json, Type targetType)
        {
            return JsonConvert.DeserializeObject(json, targetType);
        }

        public T Parse<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
