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
using Bowerbird.Core.Utilities;
using Raven.Client;
using Bowerbird.Core.Config;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;

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
        public ActionResult Create(MediaResourceCreateInput mediaResourceCreateInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(new MediaResourceCreateCommand()
            {
                Key = mediaResourceCreateInput.Key,
                MediaType = mediaResourceCreateInput.MediaType,
                UploadedOn = DateTime.UtcNow,
                UserId = _userContext.GetAuthenticatedUserId(),
                Stream = mediaResourceCreateInput.MediaType.ToLower() == "image" ? mediaResourceCreateInput.File.InputStream : null,
                OriginalFileName = mediaResourceCreateInput.MediaType.ToLower() == "image" ? mediaResourceCreateInput.OriginalFileName : null,
                VideoUri = mediaResourceCreateInput.MediaType.ToLower() == "video" ? mediaResourceCreateInput.VideoUri : null
            });

            return JsonSuccess();
        }

        /// <summary>
        /// Used to generate a Preview video by parsing the passed url and returning a blob of renderable markup
        /// 
        /// TODO: Change this to pass back a model that can be injected into an ich template instead.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult VideoPreview(string url)
        {
            string preview;

            if (_videoUtility.PreviewVideoTag(url, out preview))
            {
                return new JsonNetResult(new { success = true, PreviewTags = preview });
            }

            return new JsonNetResult(new { success = false, PreviewTags = preview });
        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult AvatarUpload(HttpPostedFileBase file)
        //{
        //    return ProcessPostedImage(string.Empty, string.Empty, file, "avatar");
        //}

        //private ActionResult ProcessPostedImage(string key, string originalFileName, HttpPostedFileBase file, string recordType, string mediaType = "image")
        //{
        //    try
        //    {
        //        var mediaResourceCreateCommand = new MediaResourceCreateCommand()
        //        {
        //            OriginalFileName = originalFileName ?? string.Empty,
        //            Stream = file.InputStream,
        //            UploadedOn = DateTime.UtcNow,
        //            Usage = recordType,
        //            UserId = _userContext.GetAuthenticatedUserId(),
        //            MediaType = mediaType,
        //            Key = key
        //        };

        //        _commandProcessor.Process(mediaResourceCreateCommand);

        //        return new JsonNetResult(new { success = true});
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonNetResult(new { success = false, error = ex.Message });
        //    }
        //}

        #endregion
    }
}