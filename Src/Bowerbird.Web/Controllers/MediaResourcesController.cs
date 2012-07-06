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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.Config;
using Bowerbird.Web.Config;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Controllers
{
    public class MediaResourcesController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IVideoUtility _videoUtility;

        #endregion

        #region Constructors

        public MediaResourcesController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext,
            IVideoUtility videoUtility
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(videoUtility, "videoUtility");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
            _videoUtility = videoUtility;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpPost]
        [Authorize]
        [Transaction]
        [ValidateInput(false)]
        public ActionResult VideoUpload(
            string Description, 
            string LinkUri, 
            string Title)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(new VideoResourceCreateCommand()
            {
                Description = Description,
                LinkUri = LinkUri,
                Title = Title,
                UploadedOn = DateTime.UtcNow,
                Usage = "video",
                UserId = _userContext.GetAuthenticatedUserId()
            });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult VideoPreview(string url)
        {
            string videoOutput;
            if (_videoUtility.PreviewVideoTag(url, out videoOutput))
            {
                return new JsonNetResult(new { success = false, EmbedTags = videoOutput });
            }

            return new JsonNetResult(new { success = false, EmbedTags = videoOutput });
        }

        [HttpPost]
        [Authorize]
        [Transaction]
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
                    UploadedOn = DateTime.UtcNow,
                    Usage = recordType,
                    UserId = _userContext.GetAuthenticatedUserId()
                };

                MediaResource mediaResource = null;

                _commandProcessor.Process<MediaResourceCreateCommand, MediaResource>(mediaResourceCreateCommand, x => { mediaResource = x; });

                _documentSession.SaveChanges();

                //string photoDateTime, photoLatitude, photoLongitude;

                //SetExifData(mediaResource, out photoDateTime, out photoLatitude, out photoLongitude);

                return new JsonNetResult(new
                    {
                        mediaResource.Id,
                        mediaResource.CreatedByUser,
                        mediaResource.Type,
                        mediaResource.UploadedOn,
                        mediaResource.Files,
                        mediaResource.Metadata,
                        Key = key

                        //PhotoDateTime = photoDateTime,
                        //PhotoLatitude = photoLatitude,
                        //PhotoLongitude = photoLongitude,
                        
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