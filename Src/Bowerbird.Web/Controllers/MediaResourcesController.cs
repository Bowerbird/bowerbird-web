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
using System.IO;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Utilities;
using Bowerbird.Web.Infrastructure;
using Raven.Client;
using Bowerbird.Core.Config;
using Bowerbird.Web.Config;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Controllers
{
    public class MediaResourcesController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MediaResourcesController(
            IMessageBus messageBus,
            IDocumentSession documentSession,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _messageBus = messageBus;
            _documentSession = documentSession;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult Create(MediaResourceCreateInput mediaResourceCreateInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, Constants.AppRootId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            var type = mediaResourceCreateInput.Type.ToLower();

            _messageBus.Send(new MediaResourceCreateCommand()
            {
                Key = mediaResourceCreateInput.Key,
                Type = type,
                Usage = mediaResourceCreateInput.Usage.ToLower(),
                UploadedOn = DateTime.UtcNow,
                UserId = _userContext.GetAuthenticatedUserId(),
                // File properties
                FileStream = type == "file" && mediaResourceCreateInput.File != null ? mediaResourceCreateInput.File.InputStream : null,
                FileName = type == "file" ? mediaResourceCreateInput.FileName : null,
                FileMimeType = type == "file" ? GetSupportedMimeType(mediaResourceCreateInput.File.InputStream, mediaResourceCreateInput.FileName, mediaResourceCreateInput.File.ContentType) : null,
                // External Video properties
                VideoProviderName = type == "externalvideo" ? mediaResourceCreateInput.VideoProviderName : null,
                VideoId = type == "externalvideo" ? mediaResourceCreateInput.VideoId : null
            });

            return JsonSuccess();
        }

        private string GetSupportedMimeType(Stream stream, string filename, string mimeType)
        {
            string foundMimeType = string.Empty;
            ImageUtility image;
            AudioUtility audio;

            if (ImageUtility.TryLoad(stream, out image))
            {
                foundMimeType = image.GetMimeType();
            }
            else if (AudioUtility.TryLoad(stream, filename, mimeType, out audio))
            {
                foundMimeType = audio.GetMimeType();
            }

            return MediaTypeUtility.GetStandardMimeTypeForMimeType(foundMimeType);
        }

        #endregion 
    }
}