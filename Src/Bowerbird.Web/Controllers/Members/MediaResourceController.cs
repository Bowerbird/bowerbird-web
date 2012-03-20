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
        public ActionResult ObservationUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "observation");
        }

        public ActionResult PostUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "post");
        }

        public ActionResult AvatarUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "user");
        }

        private ActionResult ProcessPostedImage(string key, string originalFileName, HttpPostedFileBase file, string recordType)
        {
            try
            {
                var mediaResourceCreateCommandHandler = new MediaResourceCreateCommandHandler(_documentSession, _mediaFilePathService);

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
                        id = mediaResource.Id,
                        createdByUser = mediaResource.CreatedByUser,
                        metadata = mediaResource.Metadata,
                        type = mediaResource.Type,
                        uploadedOn = mediaResource.UploadedOn,
                        // HACK
                        mediumImageUri = _mediaFilePathService.MakeMediaFileUri(mediaResource.Id, "image", "original", Path.GetExtension(mediaResource.Metadata["format"])),
                        key = key
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