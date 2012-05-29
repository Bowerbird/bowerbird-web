using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Dynamic;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Raven.Abstractions.Linq;

namespace Bowerbird.Web.Config
{
    public class ExpandoObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ExpandoObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var data = value as IDictionary<string, object>;

            var dynamicValue = ((IDynamicMetaObjectProvider)value).GetMetaObject(Expression.Constant(value));
            var xxx = dynamicValue.GetDynamicMemberNames();

            writer.WriteStartObject();
            foreach (KeyValuePair<string, object> kvp in data)
            {
                writer.WritePropertyName(kvp.Key);
                serializer.Serialize(writer, kvp.Value);
            }
            writer.WriteEndObject();

            //var dynamicValue = ((IDynamicMetaObjectProvider)value).GetMetaObject(Expression.Constant(value));

            //writer.WriteStartObject();
            //foreach (var dynamicMemberName in ((IDynamicJsonObject)value).Inner)
            //{
            //    writer.WritePropertyName(dynamicMemberName.Key);
            //    var memberValue = GetValueDynamically(value, dynamicMemberName.Value);
            //    if (memberValue == null || memberValue is ValueType || memberValue is string)
            //        writer.WriteValue(memberValue);
            //    else
            //        serializer.Serialize(writer, memberValue);
            //}
            //writer.WriteEndObject();
        }

        //private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>> callsitesCache = new ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>>();

        ///// <summary>
        ///// Gets the value dynamically.
        ///// </summary>
        ///// <param name="entity">The entity.</param>
        ///// <param name="dynamicMemberName">Name of the dynamic member.</param>
        ///// <returns></returns>
        //private static object GetValueDynamically(object entity, string dynamicMemberName)
        //{
        //    Func<string, CallSite<Func<CallSite, object, object>>> valueFactory = s => CallSite<Func<CallSite, object, object>>.Create(
        //        Binder.GetMember(
        //        CSharpBinderFlags.None,
        //        dynamicMemberName,
        //        null,
        //        new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }
        //        ));
        //    var callsite = callsitesCache.GetOrAdd(dynamicMemberName, valueFactory);

        //    return callsite.Target(callsite, entity);
        //}
    }
}