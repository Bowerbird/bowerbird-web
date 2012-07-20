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
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using NLog;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    public class AudioService : IAudioService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("AudioService");

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public AudioService(
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, MediaResource mediaResource, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).AudioServiceStatus)
            {
                failureReason = "Audio files cannot be uploaded at the moment. Please try again later.";
                return false;
            }

            try
            {
                var extension = Path.GetExtension(command.OriginalFileName) ?? string.Empty;

                if (extension.ToLower() == "mp3")
                {
                    MakeAudioMediaResourceFiles(mediaResource, command);
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving audio", exception);

                failureReason = "The file is corrupted or not a valid MP3 and could not be saved. Please check the file and try again.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        private void MakeAudioMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command)
        {
            mediaResource.AddAudioFile("Audio", command.Key);
        }

        #endregion
    }
}