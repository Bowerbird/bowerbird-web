﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Controllers
{
    public class MediaResourcesController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MediaResourcesController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
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
                MimeType = mediaResourceCreateInput.File != null ? mediaResourceCreateInput.File.ContentType : null,
                Usage = mediaResourceCreateInput.Usage,
                UploadedOn = DateTime.UtcNow,
                UserId = _userContext.GetAuthenticatedUserId(),
                Stream = mediaResourceCreateInput.File != null ? mediaResourceCreateInput.File.InputStream : null,
                OriginalFileName = mediaResourceCreateInput.OriginalFileName,
                VideoProviderName = mediaResourceCreateInput.VideoProviderName,
                VideoId = mediaResourceCreateInput.VideoId
            });

            return JsonSuccess();
        }

        #endregion
    }
}