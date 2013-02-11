using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Bowerbird.Core.ViewModels
{
    public class JsonNetResult : ActionResult
    {

        #region Members

        #endregion

        #region Constructors

        public JsonNetResult(object data)
        {
            Data = data;
        }

        #endregion

        #region Properties

        public object Data { get; set; }

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
                response.ContentType = "text/html";
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (Data != null)
            {
                // HACK: RavenDB uses an internalised version of JSON.Net 4.0.8, while Bowerbird references JSON.Net 4.5. When RavenDB returns a DESERIALISED v4.0.8 object 
                // and then I try to SERIALISE it with v4.5, the output is rubbish. For some reason JArray types don't get serialised at all.
#if JS_COMBINE_MINIFY
                var formatting = Raven.Imports.Newtonsoft.Json.Formatting.None;
#else
                var formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented;
#endif
                var writer = new Raven.Imports.Newtonsoft.Json.JsonTextWriter(response.Output) { Formatting = formatting };
                var serializer = new Raven.Imports.Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(writer, Data);
                writer.Flush();

                //var writer = new Newtonsoft.Json.JsonTextWriter(response.Output) { Formatting = Newtonsoft.Json.Formatting.Indented };
                //var serializer = new Newtonsoft.Json.JsonSerializer();
                //serializer.Serialize(writer, Data);
                //writer.Flush();
            }
        }

        #endregion

    }
}
