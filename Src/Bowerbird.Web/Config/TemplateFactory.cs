//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Nustache.Mvc;
//using System.Web;

//namespace Bowerbird.Web.Config
//{
//    public class TemplateFactory
//    {

//        #region Members

//        private readonly NustacheViewEngine _nustacheViewEngine;

//        #endregion

//        #region Constructors

//        public TemplateFactory(
//            NustacheViewEngine nustacheViewEngine)
//        {
//            _nustacheViewEngine = nustacheViewEngine;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public IHtmlString Get(string name)
//        {
//            var key = "Nustache:" + path;

//            if (_controllerContext.HttpContext.Cache[key] != null)
//            {
//                return (Template)_controllerContext.HttpContext.Cache[key];
//            }

//            var templatePath = _controllerContext.HttpContext.Server.MapPath(path);
//            var templateSource = File.ReadAllText(templatePath);
//            var template = new Template();
//            template.Load(new StringReader(templateSource));

//            _controllerContext.HttpContext.Cache.Insert(key, template, new CacheDependency(templatePath));

//            return template;
//        }

//        #endregion      
      
//    }
//}
