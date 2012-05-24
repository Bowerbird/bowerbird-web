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
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class MediaResourcesController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IConfigService _configService;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public MediaResourcesController(
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
        [Authorize]
        public ActionResult ObservationUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "observation");
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "post");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AvatarUpload(HttpPostedFileBase file)
        {
            return ProcessPostedImage(string.Empty, string.Empty, file, "avatar");
        }

        private ActionResult ProcessPostedImage(string key, string originalFileName, HttpPostedFileBase file, string recordType)
        {
            try
            {
                var mediaResourceCreateCommand = new MediaResourceCreateCommand()
                {
                    OriginalFileName = originalFileName ?? string.Empty,
                    Stream = file.InputStream,
                    UploadedOn = DateTime.Now,
                    Usage = recordType,
                    UserId = _userContext.GetAuthenticatedUserId()
                };

                MediaResource mediaResource = null;
                _commandProcessor.Process<MediaResourceCreateCommand, MediaResource>(mediaResourceCreateCommand, x => { mediaResource = x; });

                return new JsonNetResult(new
                    {
                        mediaResource.Id,
                        mediaResource.CreatedByUser,
                        mediaResource.Metadata,
                        mediaResource.Type,
                        mediaResource.UploadedOn,
                        MediumImageUri = _mediaFilePathService.MakeMediaFileUri(mediaResource, "medium"),
                        ProfileImageUri = _mediaFilePathService.MakeMediaFileUri(mediaResource, "profile"),
                        ThumbImageUri = _mediaFilePathService.MakeMediaFileUri(mediaResource, "thumbnail"),
                        Key = key
                    });
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }
}