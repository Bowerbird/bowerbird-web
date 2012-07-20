﻿using System;
using SignalR;

namespace Bowerbird.Web.Config
{
    /// <summary>
    /// The default SignalrR IJsonSerializer implementation conflicts with RavenDB's Newtonsoft JSON
    /// </summary>
    public class SignalrJsonNetSerializer : IJsonSerializer
    {
        private readonly Raven.Imports.Newtonsoft.Json.JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        public SignalrJsonNetSerializer()
            : this(new Raven.Imports.Newtonsoft.Json.JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.JsonSerializerSettings"/> to use when serializing and deserializing.</param>
        public SignalrJsonNetSerializer(Raven.Imports.Newtonsoft.Json.JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            _settings = settings;
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serailize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public string Stringify(object obj)
        {
            return Raven.Imports.Newtonsoft.Json.JsonConvert.SerializeObject(obj, Raven.Imports.Newtonsoft.Json.Formatting.None, _settings);
        }

        /// <summary>
        /// Deserializes the JSON to a .NET object.
        /// </summary>
        /// <param name="json">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public object Parse(string json)
        {
            return Raven.Imports.Newtonsoft.Json.JsonConvert.DeserializeObject(json, _settings);
        }

        /// <summary>
        /// Deserializes the JSON to a .NET object.
        /// </summary>
        /// <param name="json">The JSON to deserialize.</param>
        /// <param name="targetType">The <see cref="System.Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public object Parse(string json, Type targetType)
        {
            return Raven.Imports.Newtonsoft.Json.JsonConvert.DeserializeObject(json, targetType, _settings);
        }

        /// <summary>
        /// Deserializes the JSON to a .NET object.
        /// </summary>
        /// <typeparam name="T">The <see cref="System.Type"/> of object being deserialized.</typeparam>
        /// <param name="json">The JSON to deserialize</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public T Parse<T>(string json)
        {
            return Raven.Imports.Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
