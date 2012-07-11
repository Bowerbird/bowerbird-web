using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.ViewModels
{
    public class JsonNetResult : ActionResult
    {

        #region Members

        #endregion

        #region Constructors

        public JsonNetResult(object data)
        {
            //SerializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            Data = data;
        }

        #endregion

        #region Properties

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        //public JsonSerializerSettings SerializerSettings { get; set; }

        //public Formatting Formatting { get; set; }

        #endregion

        #region Methods

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            // Incredibly, IE *still* doesn't know what JSON is, so we have to trick it to avoid prompting the user to save the returning JSON
            if (context.RequestContext.HttpContext.Request["ie"] != null && Convert.ToBoolean(context.RequestContext.HttpContext.Request["ie"]) == true)
            {
            //}
            //if (context.RequestContext.HttpContext.Request.Browser.IsBrowser("IE"))
            //{
                response.ContentType = "text/html";
            }
            else
            {
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                //JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
                //JsonSerializer serializer = new JsonSerializer();
                //serializer.Serialize(writer, Data);
                //writer.Flush();

                // HACK: RavenDB uses an internalised version of JSON.Net 4.0.8, while Bowerbird references JSON.Net 4.5. When RavenDB returns a DESERIALISED v4.0.8 object 
                // and then I try to SERIALISE it with v4.5, the out is rubbish. For some reason JArray types don't get serialised at all.
                var writer = new Raven.Imports.Newtonsoft.Json.JsonTextWriter(response.Output) { Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented };
                var serializer = new Raven.Imports.Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }

        #endregion

    }
}
