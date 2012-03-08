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
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
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

        [HttpPost]
        public ActionResult ObservationUpload(string qqfile)
        {
            return Upload(qqfile, "observation");
        }

        public ActionResult PostUpload(string qqfile)
        {
            return Upload(qqfile, "post");
        }

        public ActionResult AvatarUpload(string qqfile)
        {
            return Upload(qqfile, "user");
        }

        private ActionResult Upload(string file, string recordType)
        {
            if (Request.Browser.IsBrowser("IE") && Request.Files != null && Request.Files[0] != null)
            {
                return ProcessPostedImage(Request.Files[0].InputStream, Request.Files[0].FileName, recordType);
            }

            if (!string.IsNullOrEmpty(file))
            {
                return ProcessPostedImage(Request.InputStream, file, recordType);
            }

            return JsonWithContentType(Json(new { success = false }, JsonRequestBehavior.AllowGet));
        }

        private ActionResult ProcessPostedImage(Stream stream, string postedFileName, string recordType)
        {
            try
            {
                var mediaResourceCreateCommandHandler = new MediaResourceCreateCommandHandler(_documentSession, _mediaFilePathService);

                var mediaResourceCreatedCommand = new MediaResourceCreateCommand()
                {
                    OriginalFileName = postedFileName,
                    Stream = stream,
                    UploadedOn = DateTime.Now,
                    Usage = recordType,
                    UserId = _userContext.GetAuthenticatedUserId()
                };

                var mediaResource = _documentSession
                    .Load<MediaResource>(
                        mediaResourceCreateCommandHandler.Handle(mediaResourceCreatedCommand)
                    );

                return new JsonNetResult(new
                    {
                        id = mediaResource.Id,
                        createdByUser = mediaResource.CreatedByUser,
                        metadata = mediaResource.Metadata,
                        type = mediaResource.Type,
                        uploadedOn = mediaResource.UploadedOn,
                        // HACK
                        mediumImageUri = _mediaFilePathService.MakeMediaFileUri(mediaResource.Id, "image", "original", Path.GetExtension(mediaResource.Metadata["format"]))
                    });

                //return JsonWithContentType(Json(new
                //{
                //    imageMediaResource.Id,
                //    imageUrl = _mediaFilePathService.MakeMediaFileUri(imageMediaResource.Id, "image", "original", Path.GetExtension(postedFileName)),
                //    fileName = postedFileName,
                //    fileSize = imageMediaResource.Metadata["size"]
                //}));

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