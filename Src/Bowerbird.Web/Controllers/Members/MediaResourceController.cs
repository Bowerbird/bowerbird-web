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
using System.Drawing;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.MediaResources;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.ImageUtilities;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class MediaResourceController : ControllerBase
    {
        #region Fields

        private readonly string _uploadsFolderRelativePath = "Media/Temp";

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IConfigService _configService;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public MediaResourceController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext,
            IConfigService configService,
            IMediaFilePathService mediaFilePathService
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
            _configService = configService;
            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ActionResult Upload(string qqfile)
        {
            if (Request.Browser.IsBrowser("IE") && Request.Files != null && Request.Files[0] != null)
            {
                return ProcessPostedImage(Request.Files[0].InputStream, Request.Files[0].FileName);
            }

            if (!string.IsNullOrEmpty(qqfile))
            {
                return ProcessPostedImage(Request.InputStream, qqfile);
            }

            return JsonWithContentType(Json(new { success = false }, JsonRequestBehavior.AllowGet));
        }

        private ActionResult ProcessPostedImage(Stream stream, string postedFileName)
        {
            try
            {
                ImageDimensions imageDimensions;
                long fileSizeInBytes;

                ImageUtility
                    .Load(stream, out fileSizeInBytes)
                    .GetImageDimensions(out imageDimensions)
                    .Cleanup();

                var imageMediaResource = new ImageMediaResource(
                            _documentSession.Load<User>(_userContext.GetAuthenticatedUserId()),
                            DateTime.Now,
                            postedFileName,
                            Path.GetExtension(postedFileName),
                            string.Empty,
                            imageDimensions.Height,
                            imageDimensions.Width
                            );

                _documentSession.Store(imageMediaResource);

                _documentSession.SaveChanges();

                //var tempFileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(postedFileName));

                ImageUtility
                    .Load(stream)
                    .GetImageDimensions(out imageDimensions)
                    //.Resize(imageApplicationMediaResourceConfig.MakeImageDimensions(), imageApplicationMediaResourceConfig.ImageDetermineBestOrientation, imageApplicationMediaResourceConfig.ImageResizeMode)
                    .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResource.Id, "image", "original", Path.GetExtension(postedFileName)))
                    .Cleanup();

                // return the imagemediaresource record id with the image url to show thumbnail
                return JsonWithContentType(Json(new
                {
                    imageMediaResource.Id,
                    imageUrl = _mediaFilePathService.MakeMediaFileUri(imageMediaResource.Id, "image", "original", Path.GetExtension(postedFileName)),
                    fileName = postedFileName,
                    fileSize = fileSizeInBytes.FileSizeDisplay()
                }));

            }
            catch (Exception ex)
            {
                return JsonWithContentType(Json(new { success = false, Error = ex.Message }));
            }
        }

        private JsonResult JsonWithContentType(JsonResult jsonResult)
        {
            if (Request.Browser.IsBrowser("IE"))
            {
                jsonResult.ContentType = "text/html";
                jsonResult.ContentEncoding = Encoding.UTF8;
            }

            return jsonResult;
        }

        #endregion
    }
}